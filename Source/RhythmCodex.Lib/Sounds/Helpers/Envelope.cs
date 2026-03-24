using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RhythmCodex.Sounds.Helpers;

/// <summary>
/// Represents a linear envelope of points.
/// </summary>
public class Envelope
{
    public Envelope(params IEnumerable<Vector2> points)
    {
        Phases = points.ToList();

        //
        // Fail-safe in case we get an empty point set.
        //

        if (Phases.Count == 0)
            Phases.Add(Vector2.Zero);
    }

    private int Phase { get; set; }
    private float PhaseProgress { get; set; }
    private List<Vector2> Phases { get; set; }

    public float Output { get; private set; }
    public int? Sustain { get; set; }

    public void SetPhase(int phase)
    {
        Phase = phase;
        PhaseProgress = 0;
    }

    public float Process(float time)
    {
        var remaining = time;

        while (remaining >= float.Epsilon)
        {
            var leftPhase = Phase;

            if (leftPhase == Sustain)
                break;
                
            if (leftPhase >= Phases.Count)
            {
                Output = Phases[^1].Y;
                break;
            }

            var rightPhase = leftPhase + 1;
            var left = Phases[leftPhase];
            var right = rightPhase >= Phases.Count ? Phases[^1] : Phases[rightPhase];

            var phaseTotalTime = right.X - left.X;
            var phaseElapsedTime = phaseTotalTime * PhaseProgress;
            var phaseRemainingTime = phaseTotalTime - phaseElapsedTime;
            
            var timeToProcess = Math.Min(remaining, phaseRemainingTime);

            if (timeToProcess >= phaseRemainingTime)
            {
                Phase = rightPhase;
                remaining -= phaseRemainingTime;
                Output = right.Y;
                continue;
            }

            if (leftPhase == Sustain)
            {
                PhaseProgress = 0;
                Output = left.Y;
            }
            else
            {
                PhaseProgress += timeToProcess / phaseTotalTime;
                Output = left.Y + PhaseProgress * (right.Y - left.Y);
                remaining -= timeToProcess;
            }
        }

        return Output;
    }

    public void Reset() =>
        Phase = 0;
}