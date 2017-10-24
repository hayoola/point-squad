using System;
using System.Runtime.Remoting.Messaging;

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
        private static readonly CPoint[] SPointsPool = new CPoint[KPoolMax];
        private static int _sPoolLen;
        private bool _isConnector;

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
                        thePoint._isConnector = true;    // Mark the point was already touched
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
            Index = index;
            Value = value;
            _isConnector = false;
        }

        public int Value { get; }

        public int Index { get; }

        public bool IsConnecting() => _isConnector;
    }

    class CSegment
    {

        public static CSegment Create(
            int[] inArray,
            ref int inStartIndex,
            ref int inEndIndex)
        {
            CSegment theSegment = null;

            if (inArray[inStartIndex] == inArray[inEndIndex])
            {
                if (inEndIndex < inArray.Length - 1)
                {
                    inEndIndex++;
                    theSegment = new CSegment(inArray, inStartIndex, inEndIndex);
                }
                    
            } else
                theSegment = new CSegment(inArray, inStartIndex, inEndIndex);

            return theSegment;
        }
        private CSegment(
            int[] inArray,
            int inStartIndex,
            int inEndIndex)
        {
            StartingPoint = CPoint.Create(inArray,inStartIndex);
            EndingPoint = CPoint.Create(inArray,inEndIndex);
        }

        public CPoint StartingPoint { get; }

        public CPoint EndingPoint { get; }

        public int Distance()
        {
            return EndingPoint.Value - StartingPoint.Value;
        }
    }

    public class CSegmentSquad
    {

        private readonly int[] _intArray;
        private CSegment[] _minimalSegments;
        private CSegment[] _subMinimalSegments;
        private int _minimalSegmentsSize, _subMinimalSegmentSize;

        public CSegmentSquad(
            int[] inArray)
        {
            _intArray = inArray;
            _minimalSegments = new CSegment[6];
            _minimalSegmentsSize = 0;
            _subMinimalSegments = new CSegment[4];
            _subMinimalSegmentSize = 0;
        }

        public bool Recruit(
            ref int index)
        {
            bool hasMore = true;



            index++;
            if (index >= _intArray.Length - 1)
                hasMore = false;

            return hasMore;
        }
    }
}