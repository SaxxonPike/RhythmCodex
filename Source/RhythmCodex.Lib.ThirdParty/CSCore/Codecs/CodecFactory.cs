﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CSCore.Codecs.AIFF;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.WAV;

namespace CSCore.Codecs
{
    /// <summary>
    ///     Helps to choose the right decoder for different codecs.
    /// </summary>
    public class CodecFactory
    {
// ReSharper disable once InconsistentNaming
        private static readonly CodecFactory _instance = new CodecFactory();

        private readonly Dictionary<object, CodecFactoryEntry> _codecs;

        private CodecFactory()
        {
            _codecs = new Dictionary<object, CodecFactoryEntry>();
            Register("wave", new CodecFactoryEntry(s =>
            {
                IWaveSource res = new WaveFileReader(s);
                if (res.WaveFormat.WaveFormatTag != AudioEncoding.Pcm &&
                    res.WaveFormat.WaveFormatTag != AudioEncoding.IeeeFloat &&
                    res.WaveFormat.WaveFormatTag != AudioEncoding.Extensible)
                {
                    throw new Exception($"Wave codec {res.WaveFormat.WaveFormatTag} not supported.");
                }
                return res;
            },
                "wav", "wave"));
            Register("flac", new CodecFactoryEntry(s => new FlacFile(s),
                "flac", "fla"));
            Register("aiff", new CodecFactoryEntry(s => new AiffReader(s),
                "aiff", "aif", "aifc"));
        }

        /// <summary>
        ///     Gets the default singleton instance of the <see cref="CodecFactory" /> class.
        /// </summary>
        public static CodecFactory Instance
        {
            get { return _instance; }
        }

        /// <summary>
        ///     Gets the file filter in English. This filter can be used e.g. in combination with an OpenFileDialog.
        /// </summary>
        public static string SupportedFilesFilterEn
        {
            get { return Instance.GenerateFilter(); }
        }

        /// <summary>
        ///     Registers a new codec.
        /// </summary>
        /// <param name="key">
        ///     The key which gets used internally to save the <paramref name="codec" /> in a
        ///     <see cref="Dictionary{TKey,TValue}" />. This is typically the associated file extension. For example: the mp3 codec
        ///     uses the string "mp3" as its key.
        /// </param>
        /// <param name="codec"><see cref="CodecFactoryEntry" /> which provides information about the codec.</param>
        public void Register(object key, CodecFactoryEntry codec)
        {
            var keyString = key as string;
            if (keyString != null)
                key = keyString.ToLower();

            if (_codecs.ContainsKey(key) != true)
                _codecs.Add(key, codec);
        }

        /// <summary>
        ///     Returns a fully initialized <see cref="IWaveSource" /> instance which is able to decode the specified file. If the
        ///     specified file can not be decoded, this method throws an <see cref="NotSupportedException" />.
        /// </summary>
        /// <param name="filename">Filename of the specified file.</param>
        /// <returns>Fully initialized <see cref="IWaveSource" /> instance which is able to decode the specified file.</returns>
        /// <exception cref="NotSupportedException">The codec of the specified file is not supported.</exception>
        public IWaveSource GetCodec(string filename)
        {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (!File.Exists(filename))
                throw new FileNotFoundException("File not found.", filename);

            string extension = Path.GetExtension(filename).Remove(0, 1); //get the extension without the "dot".

            IWaveSource source = null;
            if (File.Exists(filename))
            {
                Stream stream = File.OpenRead(filename);
                try
                {
                    foreach (var codecEntry in _codecs)
                    {
                        try
                        {
                            if (
                                codecEntry.Value.FileExtensions.Any(
                                    x => x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                            {
                                source = codecEntry.Value.GetCodecAction(stream);
                                if (source != null)
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                }
                finally
                {
                    if (source == null)
                    {
                        stream.Dispose();
                    }
                    else
                    {
                        source = new DisposeFileStreamSource(source, stream);
                    }
                }
            }

            if (source != null)
                return source;

            throw new Exception("Codec not found.");
        }

        /// <summary>
        ///     Returns a fully initialized <see cref="IWaveSource" /> instance which is able to decode the audio source behind the
        ///     specified <paramref name="uri" />.
        ///     If the specified audio source can not be decoded, this method throws an <see cref="NotSupportedException" />.
        /// </summary>
        /// <param name="uri">Uri which points to an audio source.</param>
        /// <returns>Fully initialized <see cref="IWaveSource" /> instance which is able to decode the specified audio source.</returns>
        /// <exception cref="NotSupportedException">The codec of the specified audio source is not supported.</exception>
        public IWaveSource GetCodec(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            try
            {
                var filename = TryFindFilename(uri);
                if (!String.IsNullOrEmpty(filename))
                    return GetCodec(filename);

                throw new Exception("Web streams are not supported.");
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Codec not supported.", e);
            }
        }

        internal IWaveSource GetCodec(Stream stream, object key)
        {
            return _codecs[key].GetCodecAction(stream);
        }

        /// <summary>
        ///     Returns all the common file extensions of all supported codecs. Note that some of these file extensions belong to
        ///     more than one codec.
        ///     That means that it can be possible that some files with the file extension abc can be decoded but other a few files
        ///     with the file extension abc can't be decoded.
        /// </summary>
        /// <returns>Supported file extensions.</returns>
        public string[] GetSupportedFileExtensions()
        {
            var extensions = new List<string>();
            foreach (CodecFactoryEntry item in _codecs.Select(x => x.Value))
            {
                foreach (string e in item.FileExtensions)
                {
                    if (!extensions.Contains(e))
                        extensions.Add(e);
                }
            }
            return extensions.ToArray();
        }

        private string GenerateFilter()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Supported Files|");
            stringBuilder.Append(String.Concat(GetSupportedFileExtensions().Select(x => "*." + x + ";").ToArray()));
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        private string TryFindFilename(Uri uri)
        {
            if (File.Exists(uri.LocalPath))
                return uri.LocalPath;

            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(uri.LocalPath);
                if (fileInfo.Exists)
                    return fileInfo.FullName;
            }
            catch (Exception)
            {
                Debug.WriteLine(String.Format("{0} not found.", uri.LocalPath));
            }

            try
            {
                fileInfo = new FileInfo(uri.OriginalString);
                if (fileInfo.Exists)
                    return fileInfo.FullName;
            }
            catch (Exception)
            {
                Debug.WriteLine(String.Format("{0} not found.", uri.OriginalString));
            }

            return null;
        }

        private class DisposeFileStreamSource : WaveAggregatorBase
        {
            private Stream _stream;

            public DisposeFileStreamSource(IWaveSource source, Stream stream)
                : base(source)
            {
                _stream = stream;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (_stream != null)
                {
                    try
                    {
                        _stream.Dispose();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Stream was already disposed.");
                    }
                    finally
                    {
                        _stream = null;
                    }
                }
            }
        }
    }
}