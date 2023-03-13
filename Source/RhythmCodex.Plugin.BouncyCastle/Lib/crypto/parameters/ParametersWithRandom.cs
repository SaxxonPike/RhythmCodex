using System;

namespace RhythmCodex.Plugin.BouncyCastle.Lib.crypto.parameters;

public class ParametersWithRandom
	: ICipherParameters
{
	private readonly ICipherParameters	parameters;
	private readonly Random		random;

	public ParametersWithRandom(
		ICipherParameters	parameters,
		Random		random)
	{
		if (parameters == null)
			throw new ArgumentNullException("parameters");
		if (random == null)
			throw new ArgumentNullException("random");

		this.parameters = parameters;
		this.random = random;
	}

	public ParametersWithRandom(
		ICipherParameters parameters)
		: this(parameters, new Random())
	{
	}

	[Obsolete("Use Random property instead")]
	public Random GetRandom()
	{
		return Random;
	}

	public Random Random
	{
		get { return random; }
	}

	public ICipherParameters Parameters
	{
		get { return parameters; }
	}
}