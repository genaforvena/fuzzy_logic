using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLogic_Mozerov
{
    public static class PsoMethod
    {
        public static void Process(
           List<DataSetItem> learningItemList,
           List<double>[] clusterCenterListArray,
           List<double>[] gaussWidthListArray,
           List<double> b0,
           out List<double> bestStateList,
           int particlesNumber, 
           double w,
           double c1,
           double c2,
           int maxEpoch,
           double eps)
        {
            int particleVectorSize = clusterCenterListArray.Length * clusterCenterListArray[0].Count +
                gaussWidthListArray.Length * gaussWidthListArray[0].Count + b0.Count;
            int clusterNumber = clusterCenterListArray.Length;
            int clusterVectorSize = clusterCenterListArray[0].Count;

            // create particle array
            List<double> [] particleListArray = new List<double> [particlesNumber];
            initParticles(particleListArray, clusterCenterListArray, gaussWidthListArray, b0);

            // create best particle array
            List<double>[] bestParticleStateListArray = new List<double>[particleListArray.Length];
            particleListArray.CopyTo(bestParticleStateListArray, 0);

            List<double> bestPsoStateList = new List<double>(bestParticleStateListArray[0]);
            double bestPsoValue = calcFunction(learningItemList, bestPsoStateList, clusterNumber);
            for( int particleId = 1; particleId < particlesNumber; particleId++ )
            {
                double value = calcFunction(learningItemList, particleListArray[particleId], clusterNumber);
                if( value < bestPsoValue )
                {
                    bestPsoValue = value;            
                    bestPsoStateList = new List<double>(particleListArray[particleId]);
                }
            }

            // create velocity array
            List<double> [] particlesVelocityListArray = new List<double> [particlesNumber];          
            for( int particleId = 0; particleId < particlesNumber; particleId++ )
            {
                if (particlesVelocityListArray[particleId] == null)
                {
                    particlesVelocityListArray[particleId] = new List<double>();
                }
                for( int i = 0; i < particleVectorSize; i++ )
                {
                    particlesVelocityListArray[particleId].Add(0);
                }
            }

            int epochNumber = 0;
            double error = 0.0f;
            Random rand = new Random();
            do
            {
                for( int i = 0; i < particlesNumber; i++ )
                {
            
                    for( int j = 0; j < particleVectorSize; j++ )
                    {                        
                        particlesVelocityListArray[i][j] = w * particlesVelocityListArray[i][j] + c1 * rand.NextDouble() * 
                            (bestParticleStateListArray[i][j] - particleListArray[i][j])
                            + c2 * rand.NextDouble() * (bestPsoStateList[j] - particleListArray[i][j]);
                                                
                        particleListArray[i][j] += particlesVelocityListArray[i][j];
                    }

                    double particleError = calcFunction(learningItemList, particleListArray[i], clusterNumber);
                    if( particleError < calcFunction(learningItemList, bestParticleStateListArray[i], clusterNumber) )
                    {
                        bestParticleStateListArray[i] = new List<double>(particleListArray[i]);
                    }
                    if( calcFunction(learningItemList, bestParticleStateListArray[i], clusterNumber) < calcFunction(learningItemList, bestPsoStateList, clusterNumber) )
                    {
                        bestPsoStateList =  new List<double>(bestParticleStateListArray[i]);
                    }
                }

                epochNumber++;
                error = calcFunction(learningItemList, bestPsoStateList, clusterNumber);
            } while((epochNumber < maxEpoch) && (error > eps));

            bestStateList =  bestPsoStateList;

        }

        public static double GetTestError(
            List<DataSetItem> itemList,
            List<double> particleList,
            int clusterNumber)
        {
            return calcFunction(itemList, particleList, clusterNumber);
        }

        private static double calcFunction(
            List<DataSetItem> itemList, 
            List<double> particleList, 
            int clusterNumber)
        {
            double res = 0;

            if (itemList.Count == 0)
            {
                return res;
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                double nominator = getApproxFuncFraction(itemList[i], particleList, clusterNumber, true);
                double denominator = getApproxFuncFraction(itemList[i], particleList, clusterNumber, false);
                double diff = (nominator / denominator) - itemList[i].InputYClass;
                res += diff * diff;
            }

            res /= itemList.Count;
            if (res== double.NaN)
            {
                return res;
            }
            return res;
        }

        private static void initParticles(
            List<double>[] particleListArray, 
            List<double>[] clusterCenterListArray, 
            List<double>[] gaussWidthListArray, 
            List<double> b0)
        {   
            Random rand = new Random();
            double minA = gaussWidthListArray[0][0];

            if (particleListArray[0] == null)
            {
                particleListArray[0] = new List<double>();
            }
            for( int i = 0; i < clusterCenterListArray.Length; i++ )
            {               
                for( int j = 0; j < clusterCenterListArray[0].Count; j++ )
                {
                    particleListArray[0].Add(gaussWidthListArray[i][j]);
                    particleListArray[0].Add(clusterCenterListArray[i][j]);
                }
                particleListArray[0].Add(b0[i]);
            }
           
            for( int i = 1; i < clusterCenterListArray.Length; i++ )
            {
                if( minA > gaussWidthListArray[i][0] )
                {
                    minA = gaussWidthListArray[i][0];
                }
            }

            for( int p = 1; p < particleListArray.Length; p++ )
            {
                if (particleListArray[p] == null)
                {
                    particleListArray[p] = new List<double>();
                }
                for( int k = 0; k < clusterCenterListArray.Length; k++ )
                {
                    for( int i = 0; i < clusterCenterListArray[0].Count; i++ )
                    {
                        particleListArray[p].Add(0.67f * (rand.NextDouble()) + minA);
                        particleListArray[p].Add(rand.NextDouble());
                    }
                    particleListArray[p].Add((rand.Next(int.MaxValue) % (b0.Count * b0.Count)) / (double)b0.Count);
                }
            }
        }

        private static double calcMu(
           double x,
           double c,
           double a)
        {
            double res =  Math.Exp(-(x - c) * (x - c) / (2 * a * a));           
            return res;
        }

        private static double getApproxFuncFraction(
            DataSetItem dataSetItem,
            List<double> particleList,
            int clusterNumber,
            bool isNominator)
        {
            double res = 0;           

            for (int k = 0; k < clusterNumber; k++)
            {
                double mult = 1;
                int start = k * (particleList.Count / clusterNumber);
                int end = (k + 1) * (particleList.Count / clusterNumber) - 1;
                for (int i = start; i < end; i += 2)
                {
                    mult *= calcMu(dataSetItem.InputXList[(i - start) / 2], particleList[i + 1], particleList[i]);
                    if (isNominator)
                    {
                        mult *= particleList[end];
                    }
                }               
                res += mult;
            }

            // ok
            return res;
        }
    }
}
