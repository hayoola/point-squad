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
            //  Move forward until a non-zero-distance found
            if (inArray[inStartIndex] == inArray[inEndIndex])
            {
                
                while (inEndIndex < inArray.Length - 1)
                {
                    inEndIndex++;
                    if (inArray[inStartIndex] != inArray[inEndIndex])
                    {
                        theSegment = new CSegment(inStartIndex, inEndIndex);
                        break;
                    }
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


        public int Compare(
            CSegment inSegment)
        {
            return (EndingPoint.Value - StartingPoint.Value) - 
                (inSegment.EndingPoint.Value - inSegment.StartingPoint.Value);
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
        private int _minimalSegmentsDupCount, _mSubMinimalSegmentDupCount;

        public CSegmentSquad(
            int[] inArray)
        {
            _mArray = inArray;
            _minimalSegments = new CSegment[6];
            _minimalSegmentsSize = 0;
            _subMinimalSegments = new CSegment[4];
            _subMinimalSegmentSize = 0;
            _minimalSegmentsDupCount = 0;
            _mSubMinimalSegmentDupCount = 0;
        }

        public bool Recruit(
            ref int index)
        {
            bool hasMore = true;
            int theStartIndex = index;
            int theEndIndex = index + 1;

            CSegment theSegment = CSegment.Create(_mArray, theStartIndex, ref theEndIndex);

            if (_minimalSegmentsSize == 0)
            {
                _minimalSegments[0] = theSegment;
                _minimalSegmentsSize++;
            }
            else
            {
                int theComparisonResult = theSegment.Compare(_minimalSegments[0]);
                if (theComparisonResult < 0)
                {
                    // theSegment < inArray segment

                    if( _subMinimalSegments[0] == null)
                        _subMinimalSegments[0] = _minimalSegments[0]; 
                    else
                    {
                        // Oh GOD! we should check for equality here TOO!!
                        int theSubMinimalCompareResult = _minimalSegments[0].Compare(_subMinimalSegments[0]);
                        if( theSubMinimalCompareResult < 0 )
                            _subMinimalSegments[0] = _minimalSegments[0];
                        else
                        {
                            _mSubMinimalSegmentDupCount++;
                        }
                    }


                    _minimalSegments[0] = theSegment;
                    _minimalSegmentsDupCount = 0;   // Since we've a new candidate, so reset the dup count


                } else if (theComparisonResult == 0)
                {
                    // WTF! so we need to mark this equality situation. trigger a variable to show
                    //  the possible equal values
                    _minimalSegmentsDupCount++;
                    // At the end of recruiting process, if this trigger was still set; then
                    //  we need to collect these segments...
                    // Look at FinalizeRecruit below
                }
            }

            index += theEndIndex - theStartIndex;   // Segment.create may move forward the end
            if (index >= _mArray.Length - 1)
                hasMore = false;

            return hasMore;
        }

        public bool FinalizeRecruit()
        {
            bool wasSuccessful = true;

            if (_minimalSegmentsDupCount > 0)
            {
                if (_minimalSegmentsDupCount <= 6)
                {
                    int theMinimalDistance = _minimalSegments[0].Distance(), theDistance;
                    for (int i = 0, j = 0; i < _mArray.Length - 1 || j < _minimalSegmentsDupCount; i++)
                    {
                        theDistance = _mArray[i + 1] - _mArray[i - 1];
                        if (theDistance == theMinimalDistance)
                        {
                            int theStartIndex = i, theEndIndex = i + 1;
                            CSegment theSegment = CSegment.Create(_mArray, theStartIndex, ref theEndIndex);
                            _minimalSegments[_minimalSegmentsSize] = theSegment;
                            _minimalSegmentsSize++;
                            j++;
                        }
                    }
                }
                else
                {
                    wasSuccessful = false;
                }
            }

            return wasSuccessful;
        }

        bool collectDupSegments(
            int inDupCount,
            CSegment[] inSegmentArray,
            int inMaxAllowedDup,
            ref int ioSegmentSize)
        {
            bool wasSuccessful = true;

            if (inDupCount > 0)
            {
                if (inDupCount <= inMaxAllowedDup)
                {
                    int theMinimalDistance = inSegmentArray[0].Distance(), theDistance;
                    for (int i = 0, j = 0; i < _mArray.Length - 1 || j < inDupCount; i++)
                    {
                        theDistance = _mArray[i + 1] - _mArray[i - 1];
                        if (theDistance == theMinimalDistance)
                        {
                            int theStartIndex = i, theEndIndex = i + 1;
                            CSegment theSegment = CSegment.Create(_mArray, theStartIndex, ref theEndIndex);
                            inSegmentArray[_minimalSegmentsSize] = theSegment;
                            ioSegmentSize++;
                            j++;
                        }
                    }
                }
                else
                {
                    wasSuccessful = false;
                }
            }

                return wasSuccessful;
        }
    }
}