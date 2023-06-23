using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    [Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;

        [Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return or.Any(predicate => predicate.Check(evaluators));
            }
        }

        [Serializable]
        public class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (IPredicateEvaluator evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result is null) continue;
                    if (result == negate) return false;
                }

                return true;
            }
        }

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return and.All(disjunction => disjunction.Check(evaluators));
        }
    }
}
