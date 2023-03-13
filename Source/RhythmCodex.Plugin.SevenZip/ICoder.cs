// ICoder.h

using System;

namespace RhythmCodex.Plugin.SevenZip;

public interface ICoder
{
	/// <summary>
	/// Codes streams.
	/// </summary>
	/// <param name="inStream">
	/// input Stream.
	/// </param>
	/// <param name="outStream">
	/// output Stream.
	/// </param>
	/// <param name="inSize">
	/// input Size. -1 if unknown.
	/// </param>
	/// <param name="outSize">
	/// output Size. -1 if unknown.
	/// </param>
	/// <param name="progress">
	/// callback progress reference.
	/// </param>
	/// <exception cref="SevenZip.DataErrorException">
	/// if input stream is not valid
	/// </exception>
	void Code(System.IO.Stream inStream, System.IO.Stream outStream,
		Int64 inSize, Int64 outSize, ICodeProgress progress);
};

/*
public interface ICoder2
{
	 void Code(ISequentialInStream []inStreams,
			const UInt64 []inSizes, 
			ISequentialOutStream []outStreams, 
			UInt64 []outSizes,
			ICodeProgress progress);
};
*/