using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace FuzzyLogic_Mozerov
{
    public enum MethodType
    {
        BackPropagation = 0,
        Pso = 1
    }

    public class DataSetItem
    {
        public List<double> InputXList { get; set; }
        public string InputY { get; set; }
        public int InputYClass { get; set; }
                
        public DataSetItem()
        {
            InputXList = new List<double>();
            InputY = "";
            InputYClass = -1;           
        }

        public DataSetItem(DataSetItem item)
        {
            InputXList = new List<double>(item.InputXList);
            if (item.InputXList.Count == 0)
            {
                InputYClass = -1;
            }
            InputY = item.InputY;
            InputYClass = item.InputYClass;
        }
    }

    public class DataSetClass
    {
        public List<DataSetItem> ItemList { get; set; }
        public Dictionary<string, int> YDict { get; set; }
        public int ValidationPartNumber { get; set; }

        private const int RAND_SET = 10;
        private List<double> minXCompList;
        private List<double> maxXCompList;
       

        private List<DataSetItem> [] LearningItemListArray;
        private List<DataSetItem> [] TestItemListArray;

        public DataSetClass()
        {
            ItemList = new List<DataSetItem>();
            YDict = new Dictionary<string, int>();
            ValidationPartNumber = 5;
        }

        private void createYClasses()
        {
            if (ItemList.Count > 0)
            {
                int lastInd = 0;
                foreach (var item in ItemList)
                {
                    if (!YDict.ContainsKey(item.InputY))
                    {
                        YDict.Add(item.InputY, lastInd);
                        lastInd++;
                    }
                    item.InputYClass = YDict[item.InputY];
                }
            }
        }

        private void createBinaryYClasses()
        {
            if (ItemList.Count > 0)
            {                
                foreach (var item in ItemList)
                {
                    string inputY = item.InputY;
                    if (!YDict.ContainsKey(inputY))
                    {
                        if (inputY == "0")
                        {
                            YDict.Add(inputY, 0);
                        }
                        else
                        {
                            YDict.Add(inputY, 1);
                        }
                    }
                    
                    item.InputYClass = YDict[inputY];
                }
            }
        }

        private void normalizeData()
        {
            setXBorders();

            if (ItemList.Count > 0)
            {
                foreach (var item in ItemList)
                {
                    for (int j = 0; j < item.InputXList.Count; j++)
                    {
                        item.InputXList[j] = (item.InputXList[j] - minXCompList[j]) / (maxXCompList[j] - minXCompList[j]);
                    }
                }
            }

        }

        private void setXBorders ()
        {
            if (ItemList.Count > 0)
            {
                minXCompList = new List<double>(ItemList[0].InputXList);
                maxXCompList = new List<double>(ItemList[0].InputXList);

                for (int i = 1; i < ItemList.Count; i++)
                {
                    List<double> inputXList = ItemList[i].InputXList;
                    for (int j = 0; j < inputXList.Count; j++)
                    {
                        if (minXCompList[j] > inputXList[j])
                        {
                            minXCompList[j] = inputXList[j];
                        }

                        if (maxXCompList[j] < inputXList[j])
                        {
                            maxXCompList[j] = inputXList[j];
                        }
                    }
                }
            }
        }
        
        private void createValidationSets()
        {
            List<int> indexList = new List<int>();
            Random rand = new Random();
            int ind = 0, i, j;
            int partSize = ItemList.Count / ValidationPartNumber;
            LearningItemListArray = new List<DataSetItem>[ValidationPartNumber];
            TestItemListArray = new List<DataSetItem>[ValidationPartNumber];
            for (i = 0; i < ItemList.Count; i++)
            {
                indexList.Add(i);
            }
            indexList.Shuffle(rand);
            for ( i = 0; i <= ItemList.Count - partSize; i+= partSize)
            {
                TestItemListArray[ind] = new List<DataSetItem>();
                LearningItemListArray[ind] = new List<DataSetItem>();

                for (j = 0; j < ItemList.Count; j++)
                {
                    if (j >= i && j < i + partSize)
                    {
                        TestItemListArray[ind].Add(new DataSetItem(ItemList[indexList[j]]));
                    }
                    else
                    {
                        LearningItemListArray[ind].Add(new DataSetItem(ItemList[indexList[j]]));
                    }
                }

                ind++;
            }
        }

        private void Clear()
        {
            minXCompList = null;
            maxXCompList = null;
            LearningItemListArray = null;
            TestItemListArray = null;
        }

        private void ClearAll()
        {
            ItemList.Clear();
            YDict.Clear();
            Clear();
        }
      
        public double Process( MethodType type)
        {            
            // normalize data - to [0, 1]
            normalizeData();

            // cross validation: split data into ValidationPartNumber parts, set ValidationPartNumber learning and test sets
            createValidationSets();

            ModelProcessor processor = new ModelProcessor(ValidationPartNumber, LearningItemListArray, TestItemListArray, type);
            processor.Process();

            return ( 1 - processor.AccuracyError / ValidationPartNumber ) * 100;          
        }

        public bool CreateFromFile(string fileName, bool isBinary)
        {
            ClearAll();
            if (!File.Exists(fileName))
            {
                return false;
            }
            using (TextReader reader = File.OpenText(fileName))
            {
                if (reader == null)
                {
                    return false;
                }
                
                string line = reader.ReadLine();
                while (line != null)
                {
                    try
                    {
                        DataSetItem item = new DataSetItem();
                        if (line == "")
                        {
                            line = reader.ReadLine();
                            continue;
                        }
                        string[] bits = line.Split(',');
                        for (int i = 0; i < bits.Length - 1; i++)
                        {
                            item.InputXList.Add(double.Parse(bits[i], CultureInfo.InvariantCulture));
                        }
                        item.InputY = bits[bits.Length - 1];
                        
                        // add item into list
                        ItemList.Add(item);
                    }
                    catch 
                    {}

                    // read next line
                    line = reader.ReadLine();
                }
            }

            // create Y classes
            if (isBinary)
            {
                createBinaryYClasses();
            }
            else 
            {
                createYClasses();
            }
           
            // ok
            return true;
        }
    }
}
