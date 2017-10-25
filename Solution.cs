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
        private static int[] _sBackingArray;
        private bool _isConnector;


        public static void SetBackingArray(
            int[] inArray)
        {
            _sBackingArray = inArray;
        }

        public CPoint(
            int index)
        {

            if( _sBackingArray == null )
                throw new Exception("The backing array is required!");

            Index = index;
            _isConnector = false;
            
        }

        public int Value => _sBackingArray[Index];

        public int Index { get; }

        public bool IsConnecting() => _isConnector;

        public void Touch()
        {
            if (_sBackingArray[Index] < 0) // It was already touched
                _isConnector = true;
            else
                _sBackingArray[Index] = -_sBackingArray[Index];
        }
    }

    class CSegment
    {

        public static CSegment Create(
            int[] inArray,
            int inStartIndex,
            ref int inEndIndex)
        {
            CSegment theSegment = null;

            // Handling of zero-distance segments
            if (inArray[inStartIndex] == inArray[inEndIndex])
            {
                if (inEndIndex < inArray.Length - 1)
                {
                    inEndIndex++;
                    theSegment = new CSegment(inStartIndex, inEndIndex);
                }
                    
            } else
                theSegment = new CSegment(inStartIndex, inEndIndex);

            return theSegment;
        }
        private CSegment(
            int inStartIndex,
            int inEndIndex)
        {
            StartingPoint = new CPoint(inStartIndex);
            EndingPoint = new CPoint(inEndIndex);
        }

        public static bool operator < (CSegment leftSegment, CSegment rightSegment)
        {
            return (leftSegment.EndingPoint.Value - leftSegment.StartingPoint.Value) <
                   (rightSegment.EndingPoint.Value - rightSegment.EndingPoint.Value);
        }

        public static bool operator > (CSegment leftSegment, CSegment rightSegment)
        {
            return (leftSegment.EndingPoint.Value - leftSegment.StartingPoint.Value) >
                   (rightSegment.EndingPoint.Value - rightSegment.EndingPoint.Value);
        }

        public static bool operator == (CSegment leftSegment, CSegment rightSegment)
        {
            return rightSegment != null && (leftSegment != null && (leftSegment.EndingPoint.Value - leftSegment.StartingPoint.Value) ==
                                            (rightSegment.EndingPoint.Value - rightSegment.EndingPoint.Value));
        }

        public static bool operator != (CSegment leftSegment, CSegment rightSegment)
        {
            return rightSegment != null && (leftSegment != null && (leftSegment.EndingPoint.Value - leftSegment.StartingPoint.Value) !=
                                            (rightSegment.EndingPoint.Value - rightSegment.EndingPoint.Value));
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

        private readonly int[] _mArray;
        private CSegment[] _minimalSegments;
        private CSegment[] _subMinimalSegments;
        private int _minimalSegmentsSize, _subMinimalSegmentSize;

        public CSegmentSquad(
            int[] inArray)
        {
            _mArray = inArray;
            _minimalSegments = new CSegment[6];
            _minimalSegmentsSize = 0;
            _subMinimalSegments = new CSegment[4];
            _subMinimalSegmentSize = 0;
        }

        public bool Recruit(
            ref int index)
        {
            bool hasMore = true;
            int theStartIndex = index;
            int theEndIndex = index + 1;

            CSegment theSegment = CSegment.Create(_mArray, theStartIndex, ref theEndIndex);

            if (_minimalSegmentsSize == 0)
                _minimalSegments[0] = theSegment;
            else
            {
               
            }

            index += theEndIndex - theStartIndex;   // Segment.create may move forward the end
            if (index >= _mArray.Length - 1)
                hasMore = false;

            return hasMore;
        }
    }
}