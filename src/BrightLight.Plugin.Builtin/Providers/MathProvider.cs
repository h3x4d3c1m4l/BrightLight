using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using MathNet.Symbolics;
using BrightLight.PluginInterface.Result;
using BrightLight.PluginInterface;

namespace BrightLight.Plugin.Builtin.Providers
{
    // TODO: https://github.com/mathnet/mathnet-symbolics/issues/55
    public class MathProvider : ISearchProvider
    {
        public void Dispose()
        {
        }

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            string latex;
            string infix;
            double? evaluated;

            try
            {
                // simplify
                var expression = Infix.ParseOrThrow(query.QueryString);
                latex = LaTeX.Format(expression);
                infix = Infix.Format(expression);

                // try to calculate
                var symbols = new Dictionary<string, FloatingPoint>();
                try
                {
                    evaluated = Evaluate.Evaluate(symbols, expression).RealValue;
                }
                catch (Exception ex)
                {
                    // expression valid, but can't be calculated
                    return null;
                }
            }
            catch (Exception ex)
            {
                // expression error
                return null;
            }

            var results = new SearchResultCollection
            {
                Title = "Math evaluation",
                Relevance = evaluated != null ? SearchResultRelevance.ONTOP : SearchResultRelevance.DEFAULT,
                FontAwesomeIcon = "Calculator"
            };
            results.Results = new ObservableCollection<SearchResult>
            {
                new MathSearchResult
                {
                    Title = infix,
                    LaTeXExpression = latex,
                    LaTeXEvaluated = evaluated != null ? "= " + evaluated : "",
                    ParentCollection = results
                }
            };

            return results;
        }
    }
}
