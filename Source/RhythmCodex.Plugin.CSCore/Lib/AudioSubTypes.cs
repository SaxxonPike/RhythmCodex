using System;

namespace RhythmCodex.Plugin.CSCore.Lib;

public static partial class AudioSubTypes
{
// ReSharper disable InconsistentNaming

    /// <summary>WAVE_FORMAT_UNKNOWN,   Microsoft Corporation</summary>
    public static readonly Guid Unknown = new((short)AudioEncoding.Unknown & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PCM        Microsoft Corporation</summary>
    public static readonly Guid Pcm = new((short)AudioEncoding.Pcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ADPCM      Microsoft Corporation</summary>
    public static readonly Guid Adpcm = new((short)AudioEncoding.Adpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IEEE_FLOAT Microsoft Corporation</summary>
    public static readonly Guid IeeeFloat = new((short)AudioEncoding.IeeeFloat & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VSELP      Compaq Computer Corp.</summary>
    public static readonly Guid Vselp = new((short)AudioEncoding.Vselp & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IBM_CVSD   IBM Corporation</summary>
    public static readonly Guid IbmCvsd = new((short)AudioEncoding.IbmCvsd & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ALAW       Microsoft Corporation</summary>
    public static readonly Guid ALaw = new((short)AudioEncoding.ALaw & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MULAW      Microsoft Corporation</summary>
    public static readonly Guid MuLaw = new((short)AudioEncoding.MuLaw & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DTS        Microsoft Corporation</summary>
    public static readonly Guid Dts = new((short)AudioEncoding.Dts & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DRM        Microsoft Corporation</summary>
    public static readonly Guid Drm = new((short)AudioEncoding.Drm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_WMAVOICE9 </summary>
    public static readonly Guid WmaVoice9 = new((short)AudioEncoding.WmaVoice9 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OKI_ADPCM  OKI</summary>
    public static readonly Guid OkiAdpcm = new((short)AudioEncoding.OkiAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DVI_ADPCM  Intel Corporation</summary>
    public static readonly Guid DviAdpcm = new((short)AudioEncoding.DviAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IMA_ADPCM  Intel Corporation</summary>
    public static readonly Guid ImaAdpcm = new((short)AudioEncoding.ImaAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MEDIASPACE_ADPCM Videologic</summary>
    public static readonly Guid MediaspaceAdpcm = new((short)AudioEncoding.MediaspaceAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIERRA_ADPCM Sierra Semiconductor Corp </summary>
    public static readonly Guid SierraAdpcm = new((short)AudioEncoding.SierraAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G723_ADPCM Antex Electronics Corporation </summary>
    public static readonly Guid G723Adpcm = new((short)AudioEncoding.G723Adpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIGISTD DSP Solutions, Inc.</summary>
    public static readonly Guid DigiStd = new((short)AudioEncoding.DigiStd & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIGIFIX DSP Solutions, Inc.</summary>
    public static readonly Guid DigiFix = new((short)AudioEncoding.DigiFix & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIALOGIC_OKI_ADPCM Dialogic Corporation</summary>
    public static readonly Guid DialogicOkiAdpcm = new((short)AudioEncoding.DialogicOkiAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MEDIAVISION_ADPCM Media Vision, Inc.</summary>
    public static readonly Guid MediaVisionAdpcm = new((short)AudioEncoding.MediaVisionAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CU_CODEC Hewlett-Packard Company </summary>
    public static readonly Guid CUCodec = new((short)AudioEncoding.CUCodec & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_YAMAHA_ADPCM Yamaha Corporation of America</summary>
    public static readonly Guid YamahaAdpcm = new((short)AudioEncoding.YamahaAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SONARC Speech Compression</summary>
    public static readonly Guid SonarC = new((short)AudioEncoding.SonarC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DSPGROUP_TRUESPEECH DSP Group, Inc </summary>
    public static readonly Guid DspGroupTrueSpeech = new((short)AudioEncoding.DspGroupTrueSpeech & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ECHOSC1 Echo Speech Corporation</summary>
    public static readonly Guid EchoSpeechCorporation1 = new((short)AudioEncoding.EchoSpeechCorporation1 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_AUDIOFILE_AF36, Virtual Music, Inc.</summary>
    public static readonly Guid AudioFileAf36 = new((short)AudioEncoding.AudioFileAf36 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_APTX Audio Processing Technology</summary>
    public static readonly Guid Aptx = new((short)AudioEncoding.Aptx & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_AUDIOFILE_AF10, Virtual Music, Inc.</summary>
    public static readonly Guid AudioFileAf10 = new((short)AudioEncoding.AudioFileAf10 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PROSODY_1612, Aculab plc</summary>
    public static readonly Guid Prosody1612 = new((short)AudioEncoding.Prosody1612 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_LRC, Merging Technologies S.A. </summary>
    public static readonly Guid Lrc = new((short)AudioEncoding.Lrc & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DOLBY_AC2, Dolby Laboratories</summary>
    public static readonly Guid DolbyAc2 = new((short)AudioEncoding.DolbyAc2 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_GSM610, Microsoft Corporation</summary>
    public static readonly Guid Gsm610 = new((short)AudioEncoding.Gsm610 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MSNAUDIO, Microsoft Corporation</summary>
    public static readonly Guid MsnAudio = new((short)AudioEncoding.MsnAudio & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ANTEX_ADPCME, Antex Electronics Corporation</summary>
    public static readonly Guid AntexAdpcme = new((short)AudioEncoding.AntexAdpcme & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CONTROL_RES_VQLPC, Control Resources Limited </summary>
    public static readonly Guid ControlResVqlpc = new((short)AudioEncoding.ControlResVqlpc & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIGIREAL, DSP Solutions, Inc. </summary>
    public static readonly Guid DigiReal = new((short)AudioEncoding.DigiReal & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIGIADPCM, DSP Solutions, Inc.</summary>
    public static readonly Guid DigiAdpcm = new((short)AudioEncoding.DigiAdpcm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CONTROL_RES_CR10, Control Resources Limited</summary>
    public static readonly Guid ControlResCr10 = new((short)AudioEncoding.ControlResCr10 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_NMS_VBXADPCM</summary>
    public static readonly Guid WAVE_FORMAT_NMS_VBXADPCM = new((short)AudioEncoding.WAVE_FORMAT_NMS_VBXADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CS_IMAADPCM</summary>
    public static readonly Guid WAVE_FORMAT_CS_IMAADPCM = new((short)AudioEncoding.WAVE_FORMAT_CS_IMAADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ECHOSC3</summary>
    public static readonly Guid WAVE_FORMAT_ECHOSC3 = new((short)AudioEncoding.WAVE_FORMAT_ECHOSC3 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ROCKWELL_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_ROCKWELL_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_ROCKWELL_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ROCKWELL_DIGITALK</summary>
    public static readonly Guid WAVE_FORMAT_ROCKWELL_DIGITALK = new((short)AudioEncoding.WAVE_FORMAT_ROCKWELL_DIGITALK & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_XEBEC</summary>
    public static readonly Guid WAVE_FORMAT_XEBEC = new((short)AudioEncoding.WAVE_FORMAT_XEBEC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G721_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_G721_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_G721_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G728_CELP</summary>
    public static readonly Guid WAVE_FORMAT_G728_CELP = new((short)AudioEncoding.WAVE_FORMAT_G728_CELP & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MSG723</summary>
    public static readonly Guid WAVE_FORMAT_MSG723 = new((short)AudioEncoding.WAVE_FORMAT_MSG723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MPEG, Microsoft Corporation </summary>
    public static readonly Guid Mpeg = new((short)AudioEncoding.Mpeg & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_RT24</summary>
    public static readonly Guid WAVE_FORMAT_RT24 = new((short)AudioEncoding.WAVE_FORMAT_RT24 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PAC</summary>
    public static readonly Guid WAVE_FORMAT_PAC = new((short)AudioEncoding.WAVE_FORMAT_PAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MPEGLAYER3, ISO/MPEG Layer3 Format Tag </summary>
    public static readonly Guid MpegLayer3 = new((short)AudioEncoding.MpegLayer3 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_LUCENT_G723</summary>
    public static readonly Guid WAVE_FORMAT_LUCENT_G723 = new((short)AudioEncoding.WAVE_FORMAT_LUCENT_G723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CIRRUS</summary>
    public static readonly Guid WAVE_FORMAT_CIRRUS = new((short)AudioEncoding.WAVE_FORMAT_CIRRUS & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ESPCM</summary>
    public static readonly Guid WAVE_FORMAT_ESPCM = new((short)AudioEncoding.WAVE_FORMAT_ESPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CANOPUS_ATRAC</summary>
    public static readonly Guid WAVE_FORMAT_CANOPUS_ATRAC = new((short)AudioEncoding.WAVE_FORMAT_CANOPUS_ATRAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G726_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_G726_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_G726_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G722_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_G722_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_G722_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DSAT_DISPLAY</summary>
    public static readonly Guid WAVE_FORMAT_DSAT_DISPLAY = new((short)AudioEncoding.WAVE_FORMAT_DSAT_DISPLAY & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_BYTE_ALIGNED</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_BYTE_ALIGNED = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_BYTE_ALIGNED & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_AC8</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_AC8 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_AC8 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_AC10</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_AC10 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_AC10 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_AC16</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_AC16 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_AC16 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_AC20</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_AC20 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_AC20 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_RT24</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_RT24 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_RT24 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_RT29</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_RT29 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_RT29 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_RT29HW</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_RT29HW = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_RT29HW & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_VR12</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_VR12 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_VR12 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_VR18</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_VR18 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_VR18 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_TQ40</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_TQ40 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_TQ40 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SOFTSOUND</summary>
    public static readonly Guid WAVE_FORMAT_SOFTSOUND = new((short)AudioEncoding.WAVE_FORMAT_SOFTSOUND & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VOXWARE_TQ60</summary>
    public static readonly Guid WAVE_FORMAT_VOXWARE_TQ60 = new((short)AudioEncoding.WAVE_FORMAT_VOXWARE_TQ60 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MSRT24</summary>
    public static readonly Guid WAVE_FORMAT_MSRT24 = new((short)AudioEncoding.WAVE_FORMAT_MSRT24 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G729A</summary>
    public static readonly Guid WAVE_FORMAT_G729A = new((short)AudioEncoding.WAVE_FORMAT_G729A & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MVI_MVI2</summary>
    public static readonly Guid WAVE_FORMAT_MVI_MVI2 = new((short)AudioEncoding.WAVE_FORMAT_MVI_MVI2 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DF_G726</summary>
    public static readonly Guid WAVE_FORMAT_DF_G726 = new((short)AudioEncoding.WAVE_FORMAT_DF_G726 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DF_GSM610</summary>
    public static readonly Guid WAVE_FORMAT_DF_GSM610 = new((short)AudioEncoding.WAVE_FORMAT_DF_GSM610 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ISIAUDIO</summary>
    public static readonly Guid WAVE_FORMAT_ISIAUDIO = new((short)AudioEncoding.WAVE_FORMAT_ISIAUDIO & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ONLIVE</summary>
    public static readonly Guid WAVE_FORMAT_ONLIVE = new((short)AudioEncoding.WAVE_FORMAT_ONLIVE & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SBC24</summary>
    public static readonly Guid WAVE_FORMAT_SBC24 = new((short)AudioEncoding.WAVE_FORMAT_SBC24 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DOLBY_AC3_SPDIF</summary>
    public static readonly Guid WAVE_FORMAT_DOLBY_AC3_SPDIF = new((short)AudioEncoding.WAVE_FORMAT_DOLBY_AC3_SPDIF & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MEDIASONIC_G723</summary>
    public static readonly Guid WAVE_FORMAT_MEDIASONIC_G723 = new((short)AudioEncoding.WAVE_FORMAT_MEDIASONIC_G723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PROSODY_8KBPS</summary>
    public static readonly Guid WAVE_FORMAT_PROSODY_8KBPS = new((short)AudioEncoding.WAVE_FORMAT_PROSODY_8KBPS & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ZYXEL_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_ZYXEL_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_ZYXEL_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PHILIPS_LPCBB</summary>
    public static readonly Guid WAVE_FORMAT_PHILIPS_LPCBB = new((short)AudioEncoding.WAVE_FORMAT_PHILIPS_LPCBB & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_PACKED</summary>
    public static readonly Guid WAVE_FORMAT_PACKED = new((short)AudioEncoding.WAVE_FORMAT_PACKED & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MALDEN_PHONYTALK</summary>
    public static readonly Guid WAVE_FORMAT_MALDEN_PHONYTALK = new((short)AudioEncoding.WAVE_FORMAT_MALDEN_PHONYTALK & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_GSM</summary>
    public static readonly Guid Gsm = new((short)AudioEncoding.Gsm & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G729</summary>
    public static readonly Guid G729 = new((short)AudioEncoding.G729 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G723</summary>
    public static readonly Guid G723 = new((short)AudioEncoding.G723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ACELP</summary>
    public static readonly Guid Acelp = new((short)AudioEncoding.Acelp & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     WAVE_FORMAT_RAW_AAC1
    /// </summary>
    public static readonly Guid RawAac = new((short)AudioEncoding.RawAac & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_RHETOREX_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_RHETOREX_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_RHETOREX_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IRAT</summary>
    public static readonly Guid WAVE_FORMAT_IRAT = new((short)AudioEncoding.WAVE_FORMAT_IRAT & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VIVO_G723</summary>
    public static readonly Guid WAVE_FORMAT_VIVO_G723 = new((short)AudioEncoding.WAVE_FORMAT_VIVO_G723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VIVO_SIREN</summary>
    public static readonly Guid WAVE_FORMAT_VIVO_SIREN = new((short)AudioEncoding.WAVE_FORMAT_VIVO_SIREN & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DIGITAL_G723</summary>
    public static readonly Guid WAVE_FORMAT_DIGITAL_G723 = new((short)AudioEncoding.WAVE_FORMAT_DIGITAL_G723 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SANYO_LD_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_SANYO_LD_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_SANYO_LD_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_ACEPLNET</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_ACEPLNET = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_ACEPLNET & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_ACELP4800</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_ACELP4800 = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_ACELP4800 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_ACELP8V3</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_ACELP8V3 = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_ACELP8V3 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_G729</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_G729 = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_G729 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_G729A</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_G729A = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_G729A & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SIPROLAB_KELVIN</summary>
    public static readonly Guid WAVE_FORMAT_SIPROLAB_KELVIN = new((short)AudioEncoding.WAVE_FORMAT_SIPROLAB_KELVIN & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_G726ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_G726ADPCM = new((short)AudioEncoding.WAVE_FORMAT_G726ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_QUALCOMM_PUREVOICE</summary>
    public static readonly Guid WAVE_FORMAT_QUALCOMM_PUREVOICE = new((short)AudioEncoding.WAVE_FORMAT_QUALCOMM_PUREVOICE & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_QUALCOMM_HALFRATE</summary>
    public static readonly Guid WAVE_FORMAT_QUALCOMM_HALFRATE = new((short)AudioEncoding.WAVE_FORMAT_QUALCOMM_HALFRATE & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_TUBGSM</summary>
    public static readonly Guid WAVE_FORMAT_TUBGSM = new((short)AudioEncoding.WAVE_FORMAT_TUBGSM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_MSAUDIO1</summary>
    public static readonly Guid WAVE_FORMAT_MSAUDIO1 = new((short)AudioEncoding.WAVE_FORMAT_MSAUDIO1 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Windows Media Audio, WAVE_FORMAT_WMAUDIO2, Microsoft Corporation
    /// </summary>
    public static readonly Guid WindowsMediaAudio = new((short)AudioEncoding.WindowsMediaAudio & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Windows Media Audio Professional WAVE_FORMAT_WMAUDIO3, Microsoft Corporation
    /// </summary>
    public static readonly Guid WindowsMediaAudioProfessional = new((short)AudioEncoding.WindowsMediaAudioProfessional & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Windows Media Audio Lossless, WAVE_FORMAT_WMAUDIO_LOSSLESS
    /// </summary>
    public static readonly Guid WindowsMediaAudioLosseless = new((short)AudioEncoding.WindowsMediaAudioLosseless & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Windows Media Audio Professional over SPDIF WAVE_FORMAT_WMASPDIF (0x0164)
    /// </summary>
    public static readonly Guid WindowsMediaAudioSpdif = new((short)AudioEncoding.WindowsMediaAudioSpdif & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_UNISYS_NAP_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_UNISYS_NAP_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_UNISYS_NAP_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_UNISYS_NAP_ULAW</summary>
    public static readonly Guid WAVE_FORMAT_UNISYS_NAP_ULAW = new((short)AudioEncoding.WAVE_FORMAT_UNISYS_NAP_ULAW & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_UNISYS_NAP_ALAW</summary>
    public static readonly Guid WAVE_FORMAT_UNISYS_NAP_ALAW = new((short)AudioEncoding.WAVE_FORMAT_UNISYS_NAP_ALAW & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_UNISYS_NAP_16K</summary>
    public static readonly Guid WAVE_FORMAT_UNISYS_NAP_16K = new((short)AudioEncoding.WAVE_FORMAT_UNISYS_NAP_16K & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CREATIVE_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_CREATIVE_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_CREATIVE_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CREATIVE_FASTSPEECH8</summary>
    public static readonly Guid WAVE_FORMAT_CREATIVE_FASTSPEECH8 = new((short)AudioEncoding.WAVE_FORMAT_CREATIVE_FASTSPEECH8 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CREATIVE_FASTSPEECH10</summary>
    public static readonly Guid WAVE_FORMAT_CREATIVE_FASTSPEECH10 = new((short)AudioEncoding.WAVE_FORMAT_CREATIVE_FASTSPEECH10 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_UHER_ADPCM</summary>
    public static readonly Guid WAVE_FORMAT_UHER_ADPCM = new((short)AudioEncoding.WAVE_FORMAT_UHER_ADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_QUARTERDECK</summary>
    public static readonly Guid WAVE_FORMAT_QUARTERDECK = new((short)AudioEncoding.WAVE_FORMAT_QUARTERDECK & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ILINK_VC</summary>
    public static readonly Guid WAVE_FORMAT_ILINK_VC = new((short)AudioEncoding.WAVE_FORMAT_ILINK_VC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_RAW_SPORT</summary>
    public static readonly Guid WAVE_FORMAT_RAW_SPORT = new((short)AudioEncoding.WAVE_FORMAT_RAW_SPORT & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_ESST_AC3</summary>
    public static readonly Guid WAVE_FORMAT_ESST_AC3 = new((short)AudioEncoding.WAVE_FORMAT_ESST_AC3 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IPI_HSX</summary>
    public static readonly Guid WAVE_FORMAT_IPI_HSX = new((short)AudioEncoding.WAVE_FORMAT_IPI_HSX & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_IPI_RPELP</summary>
    public static readonly Guid WAVE_FORMAT_IPI_RPELP = new((short)AudioEncoding.WAVE_FORMAT_IPI_RPELP & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_CS2</summary>
    public static readonly Guid WAVE_FORMAT_CS2 = new((short)AudioEncoding.WAVE_FORMAT_CS2 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SONY_SCX</summary>
    public static readonly Guid WAVE_FORMAT_SONY_SCX = new((short)AudioEncoding.WAVE_FORMAT_SONY_SCX & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_FM_TOWNS_SND</summary>
    public static readonly Guid WAVE_FORMAT_FM_TOWNS_SND = new((short)AudioEncoding.WAVE_FORMAT_FM_TOWNS_SND & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_BTV_DIGITAL</summary>
    public static readonly Guid WAVE_FORMAT_BTV_DIGITAL = new((short)AudioEncoding.WAVE_FORMAT_BTV_DIGITAL & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_QDESIGN_MUSIC</summary>
    public static readonly Guid WAVE_FORMAT_QDESIGN_MUSIC = new((short)AudioEncoding.WAVE_FORMAT_QDESIGN_MUSIC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VME_VMPCM</summary>
    public static readonly Guid WAVE_FORMAT_VME_VMPCM = new((short)AudioEncoding.WAVE_FORMAT_VME_VMPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_TPC</summary>
    public static readonly Guid WAVE_FORMAT_TPC = new((short)AudioEncoding.WAVE_FORMAT_TPC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OLIGSM</summary>
    public static readonly Guid WAVE_FORMAT_OLIGSM = new((short)AudioEncoding.WAVE_FORMAT_OLIGSM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OLIADPCM</summary>
    public static readonly Guid WAVE_FORMAT_OLIADPCM = new((short)AudioEncoding.WAVE_FORMAT_OLIADPCM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OLICELP</summary>
    public static readonly Guid WAVE_FORMAT_OLICELP = new((short)AudioEncoding.WAVE_FORMAT_OLICELP & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OLISBC</summary>
    public static readonly Guid WAVE_FORMAT_OLISBC = new((short)AudioEncoding.WAVE_FORMAT_OLISBC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_OLIOPR</summary>
    public static readonly Guid WAVE_FORMAT_OLIOPR = new((short)AudioEncoding.WAVE_FORMAT_OLIOPR & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_LH_CODEC</summary>
    public static readonly Guid WAVE_FORMAT_LH_CODEC = new((short)AudioEncoding.WAVE_FORMAT_LH_CODEC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_NORRIS</summary>
    public static readonly Guid WAVE_FORMAT_NORRIS = new((short)AudioEncoding.WAVE_FORMAT_NORRIS & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS</summary>
    public static readonly Guid WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS = new((short)AudioEncoding.WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Advanced Audio Coding (AAC) audio in Audio Data Transport Stream (ADTS) format.
    ///     The format block is a WAVEFORMATEX structure with wFormatTag equal to WAVE_FORMAT_MPEG_ADTS_AAC.
    /// </summary>
    /// <remarks>
    ///     The WAVEFORMATEX structure specifies the core AAC-LC sample rate and number of channels,
    ///     prior to applying spectral band replication (SBR) or parametric stereo (PS) tools, if present.
    ///     No additional data is required after the WAVEFORMATEX structure.
    /// </remarks>
    /// <see>http://msdn.microsoft.com/en-us/library/dd317599%28VS.85%29.aspx</see>
    public static readonly Guid MPEG_ADTS_AAC = new((short)AudioEncoding.MPEG_ADTS_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>MPEG_RAW_AAC</summary>
    /// <remarks>Source wmCodec.h</remarks>
    public static readonly Guid MPEG_RAW_AAC = new((short)AudioEncoding.MPEG_RAW_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     MPEG-4 audio transport stream with a synchronization layer (LOAS) and a multiplex layer (LATM).
    ///     The format block is a WAVEFORMATEX structure with wFormatTag equal to WAVE_FORMAT_MPEG_LOAS.
    ///     See <see href="http://msdn.microsoft.com/en-us/library/dd317599%28VS.85%29.aspx"/>.
    /// </summary>
    /// <remarks>
    ///     The WAVEFORMATEX structure specifies the core AAC-LC sample rate and number of channels,
    ///     prior to applying spectral SBR or PS tools, if present.
    ///     No additional data is required after the WAVEFORMATEX structure.
    /// </remarks>
    public static readonly Guid MPEG_LOAS = new((short)AudioEncoding.MPEG_LOAS & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>NOKIA_MPEG_ADTS_AAC</summary>
    /// <remarks>Source wmCodec.h</remarks>
    public static readonly Guid NOKIA_MPEG_ADTS_AAC = new((short)AudioEncoding.NOKIA_MPEG_ADTS_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>NOKIA_MPEG_RAW_AAC</summary>
    /// <remarks>Source wmCodec.h</remarks>
    public static readonly Guid NOKIA_MPEG_RAW_AAC = new((short)AudioEncoding.NOKIA_MPEG_RAW_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>VODAFONE_MPEG_ADTS_AAC</summary>
    /// <remarks>Source wmCodec.h</remarks>
    public static readonly Guid VODAFONE_MPEG_ADTS_AAC = new((short)AudioEncoding.VODAFONE_MPEG_ADTS_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>VODAFONE_MPEG_RAW_AAC</summary>
    /// <remarks>Source wmCodec.h</remarks>
    public static readonly Guid VODAFONE_MPEG_RAW_AAC = new((short)AudioEncoding.VODAFONE_MPEG_RAW_AAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     High-Efficiency Advanced Audio Coding (HE-AAC) stream.
    ///     The format block is an HEAACWAVEFORMAT structure. See <see href="http://msdn.microsoft.com/en-us/library/dd317599%28VS.85%29.aspx"/>.
    /// </summary>
    public static readonly Guid MPEG_HEAAC = new((short)AudioEncoding.MPEG_HEAAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DVM</summary>
    public static readonly Guid WAVE_FORMAT_DVM = new((short)AudioEncoding.WAVE_FORMAT_DVM & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    // others - not from MS headers
    /// <summary>WAVE_FORMAT_VORBIS1 "Og" Original stream compatible</summary>
    public static readonly Guid Vorbis1 = new((short)AudioEncoding.Vorbis1 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VORBIS2 "Pg" Have independent header</summary>
    public static readonly Guid Vorbis2 = new((short)AudioEncoding.Vorbis2 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VORBIS3 "Qg" Have no codebook header</summary>
    public static readonly Guid Vorbis3 = new((short)AudioEncoding.Vorbis3 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VORBIS1P "og" Original stream compatible</summary>
    public static readonly Guid Vorbis1P = new((short)AudioEncoding.Vorbis1P & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VORBIS2P "pg" Have independent headere</summary>
    public static readonly Guid Vorbis2P = new((short)AudioEncoding.Vorbis2P & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_VORBIS3P "qg" Have no codebook header</summary>
    public static readonly Guid Vorbis3P = new((short)AudioEncoding.Vorbis3P & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Raw AAC1
    /// </summary>
    public static readonly Guid WAVE_FORMAT_RAW_AAC1 = new((short)AudioEncoding.WAVE_FORMAT_RAW_AAC1 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    ///     Windows Media Audio Voice (WMA Voice)
    /// </summary>
    public static readonly Guid WAVE_FORMAT_WMAVOICE9 = new((short)AudioEncoding.WAVE_FORMAT_WMAVOICE9 & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>Extensible</summary>
    public static readonly Guid Extensible = new((short)AudioEncoding.Extensible & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>WAVE_FORMAT_DEVELOPMENT</summary>
    public static readonly Guid WAVE_FORMAT_DEVELOPMENT = new((short)AudioEncoding.WAVE_FORMAT_DEVELOPMENT & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
    /// <summary>
    /// FLAC
    /// </summary>
    public static readonly Guid WAVE_FORMAT_FLAC = new((short)AudioEncoding.WAVE_FORMAT_FLAC & 0x0000FFFF, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

// ReSharper restore InconsistentNaming 
}