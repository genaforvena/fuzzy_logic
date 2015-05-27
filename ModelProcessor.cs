using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FuzzyLogic_Mozerov
{
    public class ModelProcessor
    {
        public double AccuracyError { get; private set; }
        public int ClusterNumber { get; set; }
        private List<DataSetItem>[] learningItemListArray;
        private List<DataSetItem>[] testItemListArray;
        private int iterationCount;
        private MethodType methodType;

        public ModelProcessor(
            int iterCount,
            List<DataSetItem>[] learningArray,
            List<DataSetItem>[] testArray,
            MethodType type)
        {
            learningItemListArray = learningArray;
            testItemListArray = testArray;
            iterationCount = iterCount;
            AccuracyError = 0;
            ClusterNumber = 3;
            methodType = type;
        }

        public bool Process()
        {
            if (learningItemListArray.Length < iterationCount || testItemListArray.Length < iterationCount)
            {                
                return false;
            }

            AccuracyError = 0;

            for (int i = 0; i < iterationCount; i++)
            {
                List<DataSetItem> learningItemList = learningItemListArray[i];
                List<DataSetItem> testItemList = testItemListArray[i];                
                List<double>[] clusterCenterListArray = new List<double>[ClusterNumber];
                List<double>[] gaussWidthListArray = new List<double>[ClusterNumber];
                List<double> b0 = new List<double>();                
                double alphaW = 0.01;
                double alphaR = 0.005;
                const double eps = 0.0001;
                const int maxEpoch = 50;

                // perform learning
                CohonenTeacher.Teach(learningItemList, ref clusterCenterListArray, ref alphaW, ref alphaR, eps, maxEpoch);

                // create a[i,k] - gauss width
                createGaussWidth(clusterCenterListArray, ref gaussWidthListArray);

                // create b0
                createB0(learningItemList, clusterCenterListArray, gaussWidthListArray, b0);
                switch (methodType)
                {
                    case MethodType.BackPropagation:
                        const float epsBack = 0.01f;
                        const int maxEpochBack = 250;
                        float eta = 0.00007f;

                        // optmization -  back propagation
                        BackPropagationMethod.Process(learningItemList, clusterCenterListArray, gaussWidthListArray, b0, eta, maxEpochBack, epsBack);

                        // test
                        AccuracyError += BackPropagationMethod.GetTestError(testItemList, clusterCenterListArray, gaussWidthListArray, b0);

                        break;
                    case MethodType.Pso:
                    default:
                        int particlesNumber = 100;
                        double w = 0.8;
                        double c1 = 0.8;
                        double c2 = 0.8;
                        const double epsPso = 0.01;
                        const int maxEpochPso = 75;
                        List<double> bestStateList = null;

                        // optimization
                        PsoMethod.Process(learningItemList, clusterCenterListArray, gaussWidthListArray, b0, out bestStateList, particlesNumber, w, c1, c2, maxEpochPso, epsPso);

                        // test
                        AccuracyError += PsoMethod.GetTestError(testItemList, bestStateList, ClusterNumber);

                        break;
                }
            }
            // ok
            return true;
        }

        void createGaussWidth(
            List<double>[] clusterCenterList,
            ref List<double>[] gaussWidthList)
        {
            for (int i = 0; i < clusterCenterList.Length; i++)
            {
                double minDist = double.MaxValue;
                for (int j = 0; j < clusterCenterList.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    double dist = 0;
                    for (int id = 0; id < clusterCenterList[0].Count; id++)
                    {
                        dist += (clusterCenterList[i][id] - clusterCenterList[j][id]) * (clusterCenterList[i][id] - clusterCenterList[j][id]);
                    }
                    dist = Math.Sqrt(dist);

                    if (minDist > dist)
                    {
                        minDist = dist;
                    }
                }
                for (int id = 0; id < clusterCenterList[0].Count; id++)
                {
                    if (gaussWidthList[i] == null)
                    {
                        gaussWidthList[i] = new List<double>();
                    }
                    gaussWidthList[i].Add(minDist / 1.5);
                }
            }
        }

        void createB0(
            List<DataSetItem> learningItemList,
            List<double>[] clusterCenterList,
            List<double>[] gaussWidthList,
            List<double> b0)
        {
            for (int clusterId = 0; clusterId < clusterCenterList.Length; clusterId++)
            {
                double b = 0;
                double alphaSum = 0;
                for (int sampleId = 0; sampleId < learningItemList.Count; sampleId++)
                {
                    double alpha = 1; // insurance
                    for (int i = 0; i < clusterCenterList.Length; i++)
                    {
                        alpha *= Math.Exp(-(learningItemList[sampleId].InputXList[i] - clusterCenterList[clusterId][i]) * (learningItemList[sampleId].InputXList[i] - clusterCenterList[clusterId][i])
                            / (2 * gaussWidthList[clusterId][i]));
                    }
                    alphaSum += alpha;
                    b += alpha * learningItemList[sampleId].InputXList[learningItemList[sampleId].InputXList.Count - 1];
                }
                if (alphaSum != 0 || b != 0)
                {
                    b = b / alphaSum;
                }
                b0.Add(b);
            }
        }
    }
}
