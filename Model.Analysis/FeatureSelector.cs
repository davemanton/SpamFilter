using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Deedle;

namespace Model.Analysis
{
    public interface IFeatureSelector
    {
        double[][] SelectInputFeatures(Frame<int, string> frame, IEnumerable<string> terms);
        int[] SelectOutputClassifications(Frame<int, string> frame);
    }

    public class FeatureSelector : IFeatureSelector
    {
        public double[][] SelectInputFeatures(Frame<int, string> frame, IEnumerable<string> terms)
        {
            return frame.Columns[terms].Rows.Select(
                x => Array.ConvertAll<object, double>(x.Value.ValuesAll.ToArray(), Convert.ToDouble))
                .ValuesAll
                .ToArray();
        }

        public int[] SelectOutputClassifications(Frame<int, string> frame)
        {
            return frame.GetColumn<int>(DataConstants.Ham).ValuesAll.ToArray();
        }
    }
}