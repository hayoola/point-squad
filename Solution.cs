using System;
//using System.Dynamic;

namespace C1Q1
{
    public class Solution
    {
        public int Solve(int[] input)
        {
            // Implement your solution here, and return the correct result instead of 0

            int theResult = 0;

            Array.Sort(input);
            foreach (int i in input) Console.Write(i + " ");

            return theResult;
        }
    }


    class CPoint
    {
        private const int KPoolMax = 12;
        private readonly int _mValue;
        private readonly int _mIndex;
        private static readonly CPoint[] SPointsPool = new CPoint[KPoolMax];
        private static int _sPoolLen;
        

        public static CPoint Create(
            int[] inArray,
            int inIndex)
        {
            CPoint thePoint = null;

            // First check, if the point was already visted
            if (inArray[inIndex] < 0)
            {
                // Defenitly, there should be a match
                for (int i = 0; i < KPoolMax; i++)
                {
                    if (SPointsPool[i].Index == inIndex)
                    {
                        thePoint = SPointsPool[i];
                        break;
                    }

                }

                if (thePoint == null)
                {
                    throw new Exception("Oh No!, Can't find the point in the pool");
                }
            }
            else
            {
                thePoint = new CPoint(inIndex, inArray[inIndex]);
                SPointsPool[_sPoolLen] = thePoint;
                _sPoolLen++;
                if( _sPoolLen > KPoolMax)
                    throw new Exception("The Point pool is over!");
            }

            

            return thePoint;
        }

        private CPoint(
            int index,
            int value)
        {
            _mIndex = index;
            _mValue = value;
        }

        public int Value => _mValue;
        public int Index => _mIndex;
    }

}