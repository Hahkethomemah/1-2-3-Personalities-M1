using System;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace SPM1
{
    public static class DescriptionGenerator
    {
        public static string Generate(DescStack stack, Pawn pawn, int? randomSeed)
        {
            if (stack == null || pawn == null)
            {
                Core.Warn("Null stack or pawn, returning null.");
                return null;
            }

            if (randomSeed != null)
                Rand.PushState(randomSeed.Value);

            try
            {
                return stack.MakeString(pawn);
            }
            catch (Exception e)
            {
                Core.Error($"Exception generating description using stack '{stack}' for pawn '{pawn}'", e);
                return null;
            }
            finally
            {
                if (randomSeed != null)
                    Rand.PopState();
            }
        }

        public static string MakePart(string root, Pawn pawn, IEnumerable<(object, string)> toAdd)
        {
            var request = new GrammarRequest();
            foreach (var pair in toAdd)
            {
                try
                {
                    object obj = pair.Item1;
                    string name = pair.Item2;
                    switch (obj)
                    {
                        case Pawn p:
                            request.Rules.AddRange(GrammarUtility.RulesForPawn(name, p, request.Constants, true, true));
                            break;
                        case RulePackDef pack:
                            request.Includes.Add(pack);
                            break;
                        case IRuleSupplier rs:
                            request.Rules.AddRange(rs.GetRules(name, pawn));
                            break;
                        default:
                            Core.Error($"Unhandled grammar type '{obj.GetType().FullName}' ({name})");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Core.Error($"Exception adding part to grammar request: {pair.Item2} - {pair.Item1}", e);
                    throw;
                }
            }

            return GrammarResolver.Resolve(root, request)?.Trim();
        }
    }
}
