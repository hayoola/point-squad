using System;
//using System.Runtime.InteropServices;

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

            CPoint.SetBackingArray(input);
            CSegmentSquad theSegmentSquad = new CSegmentSquad(input);
            for (int i = 0; i < input.Length;)
            {
                bool canRecruit = theSegmentSquad.Recruit(ref i);
            }
            bool isFeasible = theSegmentSquad.FinalizeRecruit();

            return theResult;
        }
    
    }


    public class CPoint
    {
        private static int[] _sBackingArray;
        private int _touchCounter;
        private static readonly CPoint[] SPointsPool = new CPoint[12];
        private static int _sPointsPoolSize;


        public static void SetBackingArray(
            int[] inArray)
        {
            _sBackingArray = inArray;
            _sPointsPoolSize = 0;
        }


        public static CPoint CreateUniquePoint(
            int index)
        {
            CPoint thePoint = null;

            if (_sBackingArray[index] < 0)
            {
                // Definitly, this point was created before, so let's catch it!
                for (int i = 0; i < _sPointsPoolSize; i++)
                {
                    if (SPointsPool[i].Index == index)
                    {
                        thePoint = SPointsPool[i];
                        break;
                    }
                }

            }
            else
            {
                thePoint = new CPoint(index);
                SPointsPool[_sPointsPoolSize++] = thePoint;
                _sBackingArray[index] = -_sBackingArray[index];
            }

            return thePoint;
        }

        private CPoint(
            int index)
        {

            if( _sBackingArray == null )
                throw new Exception("The backing array is required!");

            Index = index;
            _touchCounter = 0;

        }

        public bool Touch()
        {
            bool isTouchedBefore = _touchCounter > 0;

            _touchCounter++;

            return isTouchedBefore;
        }




        public int Value => _sBackingArray[Index];

        public int Index { get; }

    }

    public class CSegment
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
            StartingPoint = CPoint.CreateUniquePoint(inStartIndex);
            EndingPoint = CPoint.CreateUniquePoint(inEndIndex);
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
            return Math.Abs(EndingPoint.Value) - Math.Abs(StartingPoint.Value);
        }
    }


    class SegmentResidency
    {
        public int Start;
        public int End;
        public int Dist;
        public int DupCount;


        public SegmentResidency()
        {
            Start = End = DupCount = 0;
            Dist = Int32.MaxValue;
        }

        public void Assign(
            SegmentResidency inResidency)
        {
            Start = inResidency.Start;
            End = inResidency.End;
            Dist = inResidency.Dist;
        }

        public static bool Inspect(
            int[] inArray,
            int inStart,
            out int outEnd,
            SegmentResidency inSegmentResidency )
        {
            bool canInspect = false;
            outEnd = inStart + 1;

            if (outEnd < inArray.Length)
            {
                if (inArray[inStart] == inArray[outEnd])
                {
                    while (outEnd < inArray.Length - 1)
                    {
                        outEnd++;
                    }
                }

                inSegmentResidency.Start = inStart;
                inSegmentResidency.End = outEnd;
                inSegmentResidency.Dist = inArray[outEnd] - inArray[inStart];
                canInspect = true;
            }

            return canInspect;
        }
    }

    public class CSegmentSquad
    {

        private readonly int[] _mArray;
        private readonly CSegment[] _minimalSegments;
        private readonly CSegment[] _subMinimalSegments;
        private int _minimalSegmentsSize, _subMinimalSegmentSize;

        private readonly SegmentResidency _minimalResidency;
        private readonly SegmentResidency _transientResidency;
        private readonly SegmentResidency _subMinimalResidency;


        public CSegmentSquad(
            int[] inArray)
        {
            _mArray = inArray;
            _minimalSegments = new CSegment[6];
            _minimalSegmentsSize = 0;
            _subMinimalSegments = new CSegment[4];
            _subMinimalSegmentSize = 0;

            _minimalResidency = new SegmentResidency();
            _transientResidency = new SegmentResidency();
            _subMinimalResidency = new SegmentResidency();
        }



        public bool Recruit(
            ref int index)
        {
            int theStartIndex = index;

            var canInspect = SegmentResidency.Inspect(_mArray, theStartIndex, out var theEndIndex, _transientResidency);
            if (canInspect)
            {
                int theComparisonResult = _transientResidency.Dist - _minimalResidency.Dist;
                if (theComparisonResult < 0)
                {
                    // Handle subMinimal here:
                    //  We put the previous minimal candidate AND its duplicate_count
                    //  into the subminimal array
                    _subMinimalResidency.Assign(_minimalResidency);
                    _subMinimalResidency.DupCount = _minimalResidency.DupCount;

                    _minimalResidency.Assign(_transientResidency);
                    _minimalResidency.DupCount = 0; // Since we've a new candidate, so reset the dup count

                }
                else if (theComparisonResult == 0)
                {
                    // WTF! so we need to mark this equality situation. trigger a variable to show
                    //  the possible equal values
                    _minimalResidency.DupCount++;
                    // At the end of recruiting process, if this trigger was still set; then
                    //  we need to collect these segments...
                    // Look at FinalizeRecruit below

                }
                else
                {
                    // The inspected segment is greater than the current minimal,
                    //  so check the subminiaml and if it is less than subminimal then
                    //  record it as the current subminimal
                    theComparisonResult = _transientResidency.Dist - _subMinimalResidency.Dist;
                    if (theComparisonResult < 0)
                    {
                        _subMinimalResidency.Assign(_transientResidency);
                        _subMinimalResidency.DupCount = 0;
                    }
                    else if (theComparisonResult == 0)
                        _subMinimalResidency.DupCount++;

                }

                index += theEndIndex - theStartIndex;
            }
            else
            {
                index++;
            }
            return canInspect;
        }

        public bool FinalizeRecruit()
        {
            // First create one segment for the minimal residency
            CSegment theSegment = CSegment.Create(_mArray, _minimalResidency.Start, ref _minimalResidency.End);
            _minimalSegments[0] = theSegment;
            _minimalSegmentsSize = 1;

            // Then handle the dups of minimal
            var wasSuccessful = _CollectDupSegments(_minimalResidency.DupCount, _minimalSegments, 5,
                ref _minimalSegmentsSize);

            // Now look at the subMinimal and if there was a valid segment, create and put it into the 
            //  subminimal segment array
            if (_subMinimalResidency.End > _subMinimalResidency.Start)
            {
                theSegment = CSegment.Create(_mArray, _subMinimalResidency.Start, ref _subMinimalResidency.End);
                _subMinimalSegments[0] = theSegment;
                _subMinimalSegmentSize = 1;
            }


            if (wasSuccessful)
                wasSuccessful = _CollectDupSegments(_subMinimalResidency.DupCount, _subMinimalSegments, 3,
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
                        var theDistance = Math.Abs(_mArray[i + 1]) - Math.Abs(_mArray[i]);
                        if (theDistance == theMinimalDistance && inSegmentArray[0].StartingPoint.Index != i
                        ) // Be careful about the same segment
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

        public CSegment[] GetMinimals()
        {
            _minimalSegments[_minimalSegmentsSize] = null;  // Nullify the end of the array
            return _minimalSegments;
        }

        public CSegment[] GetSubMinimals()
        {
            _subMinimalSegments[_subMinimalSegmentSize] = null;
            return _subMinimalSegments;
        }
}
    public enum SegmentType {
        KMinimal,
        KSubMinimal
    };

    public class CAxe
    {
        private CSegmentSquad _squad;
        private CPoint[] _pointCage;
        private int _remainingForAxe, _remainingToDepart, _remainingToKill;

        public CAxe(
            CSegmentSquad inSquad)
        {
            _squad = inSquad;
            _pointCage = new CPoint[3];
        }

        public void Arm(
            SegmentType inSegmentType)
        {
            _remainingForAxe = 3;

            if (inSegmentType == SegmentType.KMinimal)
            {
                CSegment[] theMinimals = _squad.GetMinimals();
                for (int i = 0; theMinimals[i] != null; i++)
                {
                    if (theMinimals[i].StartingPoint.Touch())
                        _remainingToDepart++;


                }
            }

            
        }

        public bool Disarmed()
        {
            return false;
        }

        public CPoint[] GetCage()
        {
            return _pointCage;
        }

        public void Depart(
            CSegment inSegment)
        {
            
        }

        public void Kill(
            CSegment inSegment)
        {
            

        }


        public void Wound(
            CSegment inSegment)
        {
            

        }
    }
}