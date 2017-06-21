using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading;
using BrightlightLib.Models;
using MathNet.Symbolics;

namespace BrightlightLib.SearchProviders
{
    public class MathProvider : ISearchProvider
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public async Task<SearchResultCollection> SearchAsync(SearchQuery query, CancellationToken ct)
        {
            string latex;
            string infix;
            double? evaluated;

            try
            {
                // simplify
                var expression = Infix.ParseOrThrow(query.Query);
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
                    evaluated = null;
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
                FontAwesomeIcon = "calculator"
            };
            results.Results = new ObservableCollection<SearchResult>
            {
                new MathResult
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
