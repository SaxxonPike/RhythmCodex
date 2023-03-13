using System;

namespace RhythmCodex.Plugin.SevenZip.Compress.RangeCoder;

struct BitTreeEncoder
{
	BitEncoder[] Models;
	int NumBitLevels;

	public BitTreeEncoder(int numBitLevels)
	{
		NumBitLevels = numBitLevels;
		Models = new BitEncoder[1 << numBitLevels];
	}

	public void Init()
	{
		for (uint i = 1; i < (1 << NumBitLevels); i++)
			Models[i].Init();
	}

	public void Encode(Encoder rangeEncoder, UInt32 symbol)
	{
		UInt32 m = 1;
		for (int bitIndex = NumBitLevels; bitIndex > 0; )
		{
			bitIndex--;
			UInt32 bit = (symbol >> bitIndex) & 1;
			Models[m].Encode(rangeEncoder, bit);
			m = (m << 1) | bit;
		}
	}

	public void ReverseEncode(Encoder rangeEncoder, UInt32 symbol)
	{
		UInt32 m = 1;
		for (UInt32 i = 0; i < NumBitLevels; i++)
		{
			UInt32 bit = symbol & 1;
			Models[m].Encode(rangeEncoder, bit);
			m = (m << 1) | bit;
			symbol >>= 1;
		}
	}

	public UInt32 GetPrice(UInt32 symbol)
	{
		UInt32 price = 0;
		UInt32 m = 1;
		for (int bitIndex = NumBitLevels; bitIndex > 0; )
		{
			bitIndex--;
			UInt32 bit = (symbol >> bitIndex) & 1;
			price += Models[m].GetPrice(bit);
			m = (m << 1) + bit;
		}
		return price;
	}

	public UInt32 ReverseGetPrice(UInt32 symbol)
	{
		UInt32 price = 0;
		UInt32 m = 1;
		for (int i = NumBitLevels; i > 0; i--)
		{
			UInt32 bit = symbol & 1;
			symbol >>= 1;
			price += Models[m].GetPrice(bit);
			m = (m << 1) | bit;
		}
		return price;
	}

	public static UInt32 ReverseGetPrice(BitEncoder[] Models, UInt32 startIndex,
		int NumBitLevels, UInt32 symbol)
	{
		UInt32 price = 0;
		UInt32 m = 1;
		for (int i = NumBitLevels; i > 0; i--)
		{
			UInt32 bit = symbol & 1;
			symbol >>= 1;
			price += Models[startIndex + m].GetPrice(bit);
			m = (m << 1) | bit;
		}
		return price;
	}

	public static void ReverseEncode(BitEncoder[] Models, UInt32 startIndex,
		Encoder rangeEncoder, int NumBitLevels, UInt32 symbol)
	{
		UInt32 m = 1;
		for (int i = 0; i < NumBitLevels; i++)
		{
			UInt32 bit = symbol & 1;
			Models[startIndex + m].Encode(rangeEncoder, bit);
			m = (m << 1) | bit;
			symbol >>= 1;
		}
	}
}