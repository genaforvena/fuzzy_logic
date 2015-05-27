using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic_Mozerov
{
    public static class CohonenTeacher
    {        
        public static void Teach(
            List<DataSetItem> LearningItemList,
            ref List<double>[] clusterCenterList, 
            ref double alphaW, 
            ref double alphaR, 
            double eps, 
            int maxEpoch)
        {
            List<int>[] winNumberList = new List<int>[LearningItemList.Count];
            Random rand = new Random();
            for (int neuronId = 0; neuronId < winNumberList.Length; neuronId++)
            {
                winNumberList[neuronId] = new List<int>();
                for (int clusterId = 0; clusterId < clusterCenterList.Length; clusterId++)
                {
                    winNumberList[neuronId].Add(1);
                }
            }

            for (int clusterId = 0; clusterId < clusterCenterList.Length; clusterId++)
            {
                clusterCenterList[clusterId] = new List<double>();                
                for (int i = 0; i < LearningItemList[0].InputXList.Count; i++)
                {
                    clusterCenterList[clusterId].Add(rand.NextDouble());
                }
            }

            for (int epochNumber = 1; epochNumber <= maxEpoch; epochNumber++)
            {
                List<double>[] previousClusterCenters = new List<double>[clusterCenterList.Length];
                clusterCenterList.CopyTo(previousClusterCenters, 0);
                for (int neuronId = 0; neuronId < LearningItemList.Count; neuronId++)
                {
                    int winnerId = -1;
                    int runnerUpId = -1;

                    // get winner and runner up
                    calcWinClusters(LearningItemList, clusterCenterList, neuronId, ref winnerId, ref runnerUpId);
                    winNumberList[neuronId][winnerId]++;

                    // correction
                    correctWeights(LearningItemList[neuronId].InputXList, clusterCenterList[winnerId], alphaW);
                    correctWeights(LearningItemList[neuronId].InputXList, clusterCenterList[runnerUpId], -alphaR);
                    correctAlpha(ref alphaW, epochNumber, maxEpoch);
                    correctAlpha(ref alphaR, epochNumber, maxEpoch);

                }

                if (getClustersDist(previousClusterCenters, clusterCenterList) < eps)
                {
                    break;
                }
            }
        }

        private static void calcWinClusters(
            List<DataSetItem> LearningItemList,
            List<double>[] clusterCenterList,
            int xInd,
            ref int wInd,
            ref int rInd)
        {
            Dictionary<int, double> diffList = new Dictionary<int, double>();

            for (int i = 0; i < clusterCenterList.Length; i++)
            {
                List<double> xList = LearningItemList[xInd].InputXList;
                List<double> centerList = clusterCenterList[i];

                diffList.Add(i, 0);

                for (int j = 0; j < centerList.Count; j++)
                {
                    diffList[i] += (centerList[j] - xList[j]) * (centerList[j] - xList[j]);
                }
            }

            diffList.OrderBy(x => x.Value);
            wInd = diffList.Last().Key;
            rInd = diffList.ElementAt(diffList.Count - 2).Key;
        }

        private static void correctWeights(
            List<double> xList,
            List<double> centerList,
            double alpha)
        {
            for (int i = 0; i < centerList.Count; i++)
            {
                centerList[i] += alpha * (xList[i] - centerList[i]);
            }
        }

        private static void correctAlpha(
            ref double alpha,
            int epochNumber,
            int maxEpoch)
        {
            alpha -= alpha * ((double)epochNumber / (double)maxEpoch);
        }

        private static double getClustersDist(
          List<double>[] prevClusterList,
          List<double>[] clusterList)
        {
            double totalSum = 0;
            for (int i = 0; i < clusterList.Length; i++)
            {
                double clusterSum = 0;
                for (int j = 0; j < clusterList[0].Count; j++)
                {
                    clusterSum += (prevClusterList[i][j] - clusterList[i][j]) * (prevClusterList[i][j] - clusterList[i][j]);
                }
                clusterSum = Math.Sqrt(clusterSum);
                totalSum += clusterSum;
            }

            return totalSum / clusterList.Length;
        }
    }
}
