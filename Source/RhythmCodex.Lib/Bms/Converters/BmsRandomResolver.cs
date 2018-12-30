using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Bms.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsRandomResolver : IBmsRandomResolver
    {
        private readonly IRandomizer _randomizer;

        public BmsRandomResolver(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public IEnumerable<BmsCommand> Resolve(IEnumerable<BmsCommand> commands)
        {
            var scope = new BmsResolverScope();
            return ResolveScope(commands, scope).ToArray();
        }

        private IEnumerable<BmsCommand> ResolveScope(IEnumerable<BmsCommand> commands, BmsResolverScope scope)
        {
            var pendingScopeCommands = new List<BmsCommand>();
            var pendingScope = new BmsResolverScope();
            var scopeLevel = 0;

            foreach (var command in commands)
            {
                // Scope management.

                if ("endif".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (scopeLevel > 0)
                    {
                        scopeLevel--;
                        if (scopeLevel == 0)
                        {
                            if (scope.RandomNumber == int.Parse(pendingScopeCommands.First().Value))
                            {
                                foreach (var innerCommand in ResolveScope(pendingScopeCommands.Skip(1), pendingScope))
                                    yield return innerCommand;
                            }
                            pendingScopeCommands.Clear();
                            pendingScope = new BmsResolverScope();
                            continue;
                        }
                    }
                }

                if ("if".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // Scope propagation.

                    if (scopeLevel == 0)
                    {
                        pendingScope.RandomNumber = scope.RandomNumber;
                    }

                    scopeLevel++;
                }

                if (scopeLevel > 0)
                {
                    pendingScopeCommands.Add(command);
                    continue;
                }

                // Scope specific commands.

                if ("random".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    scope.RandomNumber = _randomizer.GetInt(int.Parse(command.Value)) + 1;
                    continue;
                }
                
                if (scopeLevel == 0)
                    yield return command;
            }
        }
    }
}