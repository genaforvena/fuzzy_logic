using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic_Mozerov
{
    public static class BackPropagationMethod
    {
        public static void Process(
           List<DataSetItem> learningItemList,
           List<double>[] clusterCenterListArray,
           List<double>[] gaussWidthListArray,
           List<double> b0,
           double eta, 
           int maxEpoch, 
           double eps)
        {
            int epochNumber = 0;
            double error = 0.0f;
            do 
            {        
                epochNumber++;
                error = 0.0f;

                for( int i = 0; i < learningItemList.Count; i++ )
                {
                    for( int clusterInd = 0; clusterInd < clusterCenterListArray.Length; clusterInd++ )
                    {
                        List<double>[] prevClusterCenterListArray = new List<double>[clusterCenterListArray.Length];
                        List<double>[] prevGaussWidthListArray = new List<double>[gaussWidthListArray.Length];
                        List<double> prevB0 = new List<double>(b0);

                        clusterCenterListArray.CopyTo(prevClusterCenterListArray, 0);                        
                        gaussWidthListArray.CopyTo(prevGaussWidthListArray, 0);

                        for (int clusterId = 0; clusterId < clusterCenterListArray.Length; clusterId++)
                        {
                            for (int valueId = 0; valueId < clusterCenterListArray[0].Count; valueId++)
                            {
                                clusterCenterListArray[clusterId][valueId] += getDeltaC(
                                                                               learningItemList[i], 
                                                                               prevClusterCenterListArray,
                                                                               prevGaussWidthListArray, 
                                                                               prevB0, 
                                                                               eta, 
                                                                               valueId, 
                                                                               clusterId);
                                gaussWidthListArray[clusterId][valueId] += getDeltaA(
                                                                            learningItemList[i],
                                                                            prevClusterCenterListArray, 
                                                                            prevGaussWidthListArray, 
                                                                            prevB0, 
                                                                            eta, 
                                                                            valueId, 
                                                                            clusterId);
                            }
                            b0[clusterId] += getDeltaB(
                                                learningItemList[i],
                                                prevClusterCenterListArray,
                                                prevGaussWidthListArray, 
                                                prevB0, 
                                                eta, 
                                                clusterId);
                        }
                    }

                    double nominator = getApproxFuncFraction(learningItemList[i], clusterCenterListArray, gaussWidthListArray, b0, true);
                    double denominator = getApproxFuncFraction(learningItemList[i], clusterCenterListArray, gaussWidthListArray, b0, false);
                    double diff = nominator / denominator - learningItemList[i].InputYClass;
                    error += diff * diff;
                }               
                error /= (double)learningItemList.Count;
            } while( (epochNumber <= maxEpoch) && (error > eps) );
        }

        public static double GetTestError(
           List<DataSetItem> testItemList,
           List<double>[] clusterCenterListArray,
           List<double>[] gaussWidthListArray,
           List<double> b0)
        {
            // test error
            double error = 0;
            for (int i = 0; i < testItemList.Count; i++)
            {
                double nominator = getApproxFuncFraction(testItemList[i], clusterCenterListArray, gaussWidthListArray, b0, true);
                double denominator = getApproxFuncFraction(testItemList[i], clusterCenterListArray, gaussWidthListArray, b0, false);
                double diff = nominator / denominator - testItemList[i].InputYClass;
                error += diff * diff;               
            }
            error /= testItemList.Count;

            return error;
        }

        private static double calcMu(
           double x,
           double c,
           double a)
        {
            return Math.Exp(-(x - c) * (x - c) / (2 * a * a));
        }

        private static double getApproxFuncFraction(
            DataSetItem dataSetItem,
            List<double>[] clusterCenterListArray,
            List<double>[] gaussWidthListArray,
            List<double> b0,
            bool isNominator)
        {
            double res = 0;

            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                (b0 != null && clusterCenterListArray.Length != b0.Count))
            {
                // err
                return double.NaN;
            }

            for (int k = 0; k < clusterCenterListArray.Length; k++ )
            {
                double mult = 1;
                for (int i = 0; i < dataSetItem.InputXList.Count; i++)
                {
                    mult *= calcMu(dataSetItem.InputXList[i], clusterCenterListArray[k][i], gaussWidthListArray[k][i]);
                    if (isNominator)
                    {
                        mult *= b0[k];
                    }
                }
               
                res += mult;
            }

            // ok
            return res;
        }


        private static double getApproxFuncFractionClustDeriv(
            DataSetItem dataSetItem,
            List<double>[] clusterCenterListArray,
            List<double>[] gaussWidthListArray,
            List<double> b0,
            int i,
            int k,
            bool isNominator)
        {
            double res = 1;
            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                (b0 != null && clusterCenterListArray.Length != b0.Count))
            {
                // err
                return double.NaN;
            }           
            for (int j = 0; j < dataSetItem.InputXList.Count; j++)
            {
                res *= calcMu(dataSetItem.InputXList[j], clusterCenterListArray[k][j], gaussWidthListArray[k][j]);
                if (isNominator)
                {
                    res *= b0[k];
                }
                if (j == i)
                {
                    res *= (dataSetItem.InputXList[i] - clusterCenterListArray[k][j]) / (gaussWidthListArray[k][j] * gaussWidthListArray[k][j] );
                }
            }
            /*
            if (isNominator)
            {
                res *= b0[k];
            }
             * */
            return res;
        }

        private static double getApproxFuncFractionGaussDeriv(
            DataSetItem dataSetItem,
            List<double>[] clusterCenterListArray,
            List<double>[] gaussWidthListArray,
            List<double> b0,
            int i,
            int k,
            bool isNominator)
        {
            double res = 1;
            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                (b0 != null && clusterCenterListArray.Length != b0.Count))
            {
                // err
                return double.NaN;
            }
            for (int j = 0; j < dataSetItem.InputXList.Count; j++)
            {
                res *= calcMu(dataSetItem.InputXList[j], clusterCenterListArray[k][j], gaussWidthListArray[k][j]);
                if (isNominator)
                {
                    res *= b0[k];
                }
                if (j == i)
                {
                    res *= (dataSetItem.InputXList[i] - clusterCenterListArray[k][j]) * (dataSetItem.InputXList[i] - clusterCenterListArray[k][j])
                                                / (gaussWidthListArray[k][j] * gaussWidthListArray[k][j] * gaussWidthListArray[k][j]);
                }
            }
            /*
            if (isNominator)
            {
                res *= b0[k];
            }
            */
            return res;
        }


        private static double getDeltaA(
            DataSetItem dataSetItem, 
            List<double>[] clusterCenterListArray, 
            List<double>[] gaussWidthListArray, 
            List<double> b0, 
            double eta, 
            int i, 
            int k)
        {
            double res = 0;

            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                 clusterCenterListArray.Length != b0.Count)
            {
                // err
                return double.NaN;
            }   

            double nominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, true);
            double denominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, false);
            double nomDer = getApproxFuncFractionGaussDeriv(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, i, k, true);
            double denomDer = getApproxFuncFractionGaussDeriv(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, i, k, false);
            double deriv = (nomDer * denominator - nominator * denomDer) / (denominator * denominator);
            res = 2 * (nominator / denominator - dataSetItem.InputYClass) * deriv;
            
            // ok 
            return - eta * res;
        }

        private static double getDeltaC(
            DataSetItem dataSetItem,
            List<double>[] clusterCenterListArray,
            List<double>[] gaussWidthListArray,
            List<double> b0,
            double eta,
            int i,
            int k)
        {
            double res = 0;

            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                 clusterCenterListArray.Length != b0.Count)
            {
                // err
                return double.NaN;
            }

            double nominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, true);
            double denominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, false);
            double nomDer = getApproxFuncFractionClustDeriv(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, i, k, true);
            double denomDer = getApproxFuncFractionClustDeriv(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, i, k, false);
            double deriv = (nomDer * denominator - nominator * denomDer) / (denominator * denominator);
            res = 2 * (nominator / denominator - dataSetItem.InputYClass) * deriv;

            // ok 
            return -eta * res;
        }

        private static double getDeltaB(
            DataSetItem dataSetItem,
            List<double>[] clusterCenterListArray,
            List<double>[] gaussWidthListArray,
            List<double> b0,
            double eta,            
            int k)
        {
            double res = 0;

            if (clusterCenterListArray.Length != gaussWidthListArray.Length ||
                 clusterCenterListArray.Length != b0.Count)
            {
                // err
                return double.NaN;
            }

            double nominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, true);
            double denominator = getApproxFuncFraction(dataSetItem, clusterCenterListArray, gaussWidthListArray, b0, false);
            double deriv = 1;
            for (int j = 0; j < dataSetItem.InputXList.Count; j++)
            {
                deriv *= calcMu(dataSetItem.InputXList[j], clusterCenterListArray[k][j], gaussWidthListArray[k][j]);
            }
            deriv /= denominator;
           
            res = 2 * (nominator / denominator - dataSetItem.InputYClass) * deriv;

            // ok 
            return - eta * res;
        }
    }
}
