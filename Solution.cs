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
        private readonly CSegment[] _minimalSegments;
        private readonly CSegment[] _subMinimalSegments;
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
                if (theComparisonResult < 0)  // theSegment < inArray_segment if_branch
                {
                    
                    // Handle subMinimal here:
                    //  We put the previous minimal candidate AND its duplicate_count
                    //  into the subminimal array
                    _subMinimalSegments[0] = _minimalSegments[0];
                    _mSubMinimalSegmentDupCount = _minimalSegmentsDupCount;


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
            var wasSuccessful = _CollectDupSegments(_minimalSegmentsDupCount, _minimalSegments, 6, 
                ref _minimalSegmentsSize);

            if (wasSuccessful)
                wasSuccessful = _CollectDupSegments(_mSubMinimalSegmentDupCount, _subMinimalSegments, 4,
                    ref _subMinimalSegmentSize);

            return wasSuccessful;
        }

        private bool _CollectDupSegments(
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
                    int theMinimalDistance = inSegmentArray[0].Distance();
                    for (int i = 0, j = 0; i < _mArray.Length - 1 || j < inDupCount; i++)
                    {
                        var theDistance = _mArray[i + 1] - _mArray[i - 1];
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