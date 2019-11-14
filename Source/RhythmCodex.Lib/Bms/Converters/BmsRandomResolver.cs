using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Bms.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

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

        public IList<BmsCommand> Resolve(IEnumerable<BmsCommand> commands)
        {
            var scope = new BmsResolverScope();
            return ResolveScope(commands, scope, true).ToList();
        }

        private IEnumerable<BmsCommand> ResolveScope(IEnumerable<BmsCommand> commands, BmsResolverScope scope,
            bool isRootScope)
        {
            var pendingScopeCommands = new List<BmsCommand>();
            var pendingScope = new BmsResolverScope();
            var scopeLevel = 0;

            foreach (var command in commands)
            {
                // Ending any block will evaluate it when the scope level returns to zero.
                
                if ("endif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                    "endsw".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                    "elseif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                    "else".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (scopeLevel > 0)
                    {
                        scopeLevel--;
                        if (scopeLevel == 0)
                        {
                            var topCommand = pendingScopeCommands.First();
                            if ("switch".Equals(topCommand.Name, StringComparison.OrdinalIgnoreCase) || 
                                (topCommand.Value == null && !scope.Satisfied) || 
                                (topCommand.Value != null && scope.CompareValue == topCommand.Value))
                            {
                                scope.Satisfied = true;
                                var innerScopeCommands = pendingScopeCommands.Skip(1).ToArray();
                                Console.WriteLine("Processing inner scope:");
                                Console.WriteLine(string.Join(Environment.NewLine, innerScopeCommands.Select(isc => $"{isc}")));
                                var innerScopeOutput =
                                    ResolveScope(innerScopeCommands, pendingScope, false).ToArray();
                                foreach (var innerCommand in innerScopeOutput)
                                    yield return innerCommand;
                            }

                            pendingScopeCommands.Clear();
                            pendingScope = new BmsResolverScope();
                        }

                        if ("endif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                            "endsw".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            scope.Satisfied = false;
                            if (scopeLevel == 0)
                                continue;
                        }
                    }
                }
                
                // Starting a block will prepare a scope.

                if ("if".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                    "elseif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                    "else".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (scopeLevel == 0)
                    {
                        pendingScope.CompareValue = scope.CompareValue;
                        pendingScope.Matched = true;
                    }
                    scopeLevel++;
                }

                if ("switch".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (scopeLevel == 0)
                    {
                        pendingScope.CompareValue = $"{_randomizer.GetInt(int.Parse(command.Value)) + 1}";
                        pendingScope.Matched = false;
                    }
                    scopeLevel++;
                }
                
                // Skip scope processing if we are figuring out an inner scope.

                if (scopeLevel > 0)
                {
                    pendingScopeCommands.Add(command);
                    continue;
                }

                // Scope specific commands.

                if (!isRootScope)
                {
                    if ("case".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (command.Value == scope.CompareValue)
                            scope.Matched = true;
                        continue;
                    }

                    if ("def".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        scope.Matched = true;
                        continue;
                    }

                    if (scope.Matched && "skip".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                        yield break;
                }
                
                if ("random".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    scope.CompareValue = $"{_randomizer.GetInt(int.Parse(command.Value)) + 1}";
                    continue;
                }

                if ("setrandom".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    scope.CompareValue = command.Value;
                    continue;
                }

                if ("endrandom".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    scope.CompareValue = "1";
                    continue;
                }

                if (isRootScope || scope.Matched)
                    yield return command;
            }
        }
    }
}