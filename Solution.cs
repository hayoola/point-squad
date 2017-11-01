using System;
using System.Security.Cryptography;


namespace C1Q1
{
    public class Solution
    {
        public int Solve(int[] input)
        {
            // Implement your solution here, and return the correct result instead of 0

            int theResult = 0;

            if (input.Length < 5)
                return 0;

            Array.Sort(input);
            foreach (int i in input) Console.Write(i + " ");

            CPoint.SetBackingArray(input);
            CSegmentSquad theSegmentSquad = new CSegmentSquad(input);
            for (int i = 0; i < input.Length;)
            {
                bool canRecruit = theSegmentSquad.Recruit(ref i);
            }
            bool isFeasible = theSegmentSquad.FinalizeRecruit();

            if (!isFeasible)
                return 0;

            CAxe theAxe = new CAxe( theSegmentSquad);
            theSegmentSquad.Arm();

            CSegmentPool theMinimals = theSegmentSquad.GetMinimals();
            CSegmentPool theSubMinimals = theSegmentSquad.GetSubMinimals();
            CSegmentPool thePool = theMinimals;
            while (theAxe.CanAttack())
            {
                if (!theAxe.Attack(thePool))
                    break;

                if (thePool == theMinimals)
                {
                    if (theSubMinimals.GetMinDist() < thePool.GetMinDist())
                        thePool = theSubMinimals;
                }
                else
                {
                    if (theMinimals.GetMinDist() < theSubMinimals.GetMinDist())
                        thePool = theMinimals;
                }
            }

            if (theSegmentSquad.Disarmed())
            {
                CPoint[] theCage = theAxe.GetCage();
                foreach (var thePoint in theCage)
                {
                    theResult += Math.Abs(thePoint.Value);
                }
            }

            return theResult;
        }
    
    }


    public class CPoint
    {
        private static int[] _sBackingArray;
        private int _touchCounter;
        private static readonly CPoint[] SPointsPool = new CPoint[12];
        private static int _sPointsPoolSize;
        private bool _isInfected;
        private bool _isHunted;


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
                        thePoint.Touch();
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
            _isInfected = false;
            _isHunted = false;
        }

        public bool Touch()
        {
            bool isTouchedBefore = _touchCounter > 0;

            _touchCounter++;

            return isTouchedBefore;
        }

        public bool IsTouched()
        {
            return _touchCounter > 0;
        }

        public void Infect()
        {
            _isInfected = true;
        }

        public bool IsInfected()
        {
            return _isInfected;
        }

        public int Value => _sBackingArray[Index];

        public int Index { get; }

        public void Hunt()
        {
            _isHunted = true;
        }

        public bool IsHunted()
        {
            return _isHunted;
        }
        
        public int PrevDistance()
        {
            int theDistance = 0;

            if (Index > 0)
                theDistance = Math.Abs(_sBackingArray[Index] - _sBackingArray[Index - 1]);
            
            return theDistance;
        }
        
        public int NextDistance()
        {
            int theDistance = 0;

            if (Index < _sBackingArray.Length - 1)
                theDistance = Math.Abs(_sBackingArray[Index + 1] - _sBackingArray[Index]);
            
            return theDistance;
        }

    }

    public class CSegment
    {

        private int _distance;
        private bool[] _injuryRegistry;
        
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
            _distance = Math.Abs(EndingPoint.Value) - Math.Abs(StartingPoint.Value);
            _injuryRegistry = new bool[2];
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
            return _distance;
        }
        
        public void NeighborDistances( 
            out int outStartingDistance, 
            out int outEndingDistance)
        {
            outStartingDistance = StartingPoint.PrevDistance();
            outEndingDistance = EndingPoint.NextDistance();
        }

        public CPoint Injure(
            CSegmentPool inOwningPool,
            int inPoolIdx)
        {
            CPoint thePointToHunt = null, thePointToInfect = null;
            int whichEndToHunt = 0;  // Init to 'no end'. 1: starting, 2: ending

            // Prefere to depart, so first check if there is any touched points?
            // It's NOT good, because we need to look forward and if there is the possiblility
            //      for moving forward, then try to avoid the point, which was shared by the
            //      prev injured segment!!
            // So if we can move forward ? How we can check about this fact? 
            //      (use index to see if this is the last point
            //just check the prev segment and 
            // NO No NO!!!!
            // We need to introduce a new concept: InfectedPoint
            //  In a segment, when we haunt an end_point then the other point is 'Infected'
            //      and when we are checking for touch_points, then we choose them only if
            //      it is NOT infected
            if (StartingPoint.IsTouched() && !StartingPoint.IsInfected())
            {
                

                thePointToHunt = StartingPoint;
                thePointToInfect = EndingPoint;
                whichEndToHunt = 1;
            }
            else if (EndingPoint.IsTouched())
            {
                thePointToHunt = EndingPoint;
                whichEndToHunt = 2;
                thePointToInfect = StartingPoint;
            }
            else
            {
                // Do the wounding process
                // Look at the neighbors:
                NeighborDistances(out var theStartPrevDist, out var theEndNextDist);
                if( theStartPrevDist == theEndNextDist )
                {
                    // Since the points are sorted, so the starting_point < ending (always!)
                    thePointToHunt = StartingPoint;
                    whichEndToHunt = 1;
                    thePointToInfect = EndingPoint;


                }
                else if( theStartPrevDist < theEndNextDist )
                {
                    thePointToHunt = EndingPoint;
                    whichEndToHunt = 2;
                    thePointToInfect = StartingPoint;
                }
                else // theStartPrevDist > theEndNextDist
                {
                    thePointToHunt = StartingPoint;
                    whichEndToHunt = 1;
                    thePointToInfect = EndingPoint;
                }
            }

            thePointToHunt?.Hunt();
            thePointToInfect?.Infect();
            
            // Now recalculate the distance, considering the hunted point
            // But what if all two points was haunted?! Hey: The Distance = PrevDist + ThisDist + NextDist
            // Determine the most recently hunted point and then add its relevant neighbor to the distance
            
            switch( whichEndToHunt )
            {
                case 1:
                    _distance += StartingPoint.PrevDistance();
                    _injuryRegistry[0] = true;
                    break;
                    
                    
                case 2:
                    _distance += EndingPoint.NextDistance();
                    _injuryRegistry[1] = true;
                    break;
            }


            return thePointToHunt;
        }

        public bool WasInjured()
        {
            var theResult = StartingPoint.IsHunted() || EndingPoint.IsHunted();

            if (StartingPoint.IsHunted() && ! _injuryRegistry[0])
            {
                _distance += StartingPoint.PrevDistance();
                _injuryRegistry[0] = true;

            }

            if (EndingPoint.IsHunted() && ! _injuryRegistry[1])
            {
                _distance += EndingPoint.NextDistance();
                _injuryRegistry[1] = true;
            }

            return theResult;
        } 
        
    }


    public class CSegmentPool
    {
        private readonly CSegment[] _segments;
        private int _size;
        private int _minDist;
        private int _minSegmentIdx;

        public CSegmentPool()
        {
            _segments = new CSegment[6];
            _size = 0;
            _minDist = Int32.MaxValue;
            _minSegmentIdx = 0;
        }

        public void Add(
            CSegment inSegment)
        {
            if( _size > 6 )
                throw new Exception("Segment Pool is full!");

            _segments[_size++] = inSegment;
            if (inSegment.Distance() < _minDist)
                _minDist = inSegment.Distance();
        }

        public int GetMinDist()
        {
            return _minDist;
        }


        public CSegment GetSegment(
            int inIdx)
        {
            return _segments[inIdx];
        }

        public CPoint Injure()
        {
            CPoint theHuntedpoint = null;

            if (_segments[_minSegmentIdx].WasInjured())
                _reCalculateMinDistance();

            theHuntedpoint = _segments[_minSegmentIdx].Injure(this, _minSegmentIdx);

            // Now we should recalculate the min_distnace considering this injury!
            // But how?!
            // Whenever a segment was injured, we should re-calculate the distance
            _reCalculateMinDistance();

            return theHuntedpoint;
        }

        private void _reCalculateMinDistance()
        {
            _minDist = Int32.MaxValue; // Reset 
            for (int i = 0; i < _size; i++)
            {
                if (_segments[i].Distance() < _minDist)
                {
                    _minDist = _segments[i].Distance();
                    _minSegmentIdx = i;
                }
            }
        }

        public CSegment _minSegment()
        {
            return _segments[_minSegmentIdx];
        }

        public int Size()
        {
            return _size;
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


        private readonly CSegmentPool _minialSegmentPool;
        private readonly CSegmentPool _subMinimalSegmentPool;

        private readonly SegmentResidency _minimalResidency;
        private readonly SegmentResidency _transientResidency;
        private readonly SegmentResidency _subMinimalResidency;


        private int _requiredSegmentsToInjure;
        private int _potentialSegmentsToInjure;


        public CSegmentSquad(
            int[] inArray)
        {
            _mArray = inArray;

            _minialSegmentPool = new CSegmentPool();
            _subMinimalSegmentPool = new CSegmentPool();

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
            _minialSegmentPool.Add(theSegment);

            // Then handle the dups of minimal
            var wasSuccessful = _CollectDupSegments(_minimalResidency.DupCount, _minialSegmentPool, 5);

            // Now look at the subMinimal and if there was a valid segment, create and put it into the 
            //  subminimal segment array
            if (_subMinimalResidency.End > _subMinimalResidency.Start)
            {
                theSegment = CSegment.Create(_mArray, _subMinimalResidency.Start, ref _subMinimalResidency.End);
                _subMinimalSegmentPool.Add(theSegment);
            }


            if (wasSuccessful)
                wasSuccessful = _CollectDupSegments(_subMinimalResidency.DupCount, _subMinimalSegmentPool, 3);

            return wasSuccessful;
        }

        

        private bool _CollectDupSegments(
            int inDupCount,
            CSegmentPool inSegmentPool,
            int inMaxAllowedDup)
        {
            bool wasSuccessful = true;

            if (inDupCount > 0)
            {
                if (inDupCount <= inMaxAllowedDup)
                {
                    int theMinimalDistance = inSegmentPool.GetMinDist();
                    for (int i = 0, j = 0; i < _mArray.Length - 1 || j < inDupCount; i++)
                    {
                        var theDistance = Math.Abs(_mArray[i + 1]) - Math.Abs(_mArray[i]);
                        if (theDistance == theMinimalDistance && inSegmentPool.GetSegment(0).StartingPoint.Index != i
                        ) // Be careful about the same segment
                        {
                            int theStartIndex = i, theEndIndex = i + 1;
                            CSegment theSegment = CSegment.Create(_mArray, theStartIndex, ref theEndIndex);
                            inSegmentPool.Add(theSegment);
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

        public CSegmentPool GetMinimals()
        {
            
            return _minialSegmentPool;
        }

        public CSegmentPool GetSubMinimals()
        {
            return _subMinimalSegmentPool;
        }

        public void Arm()
        {


            _requiredSegmentsToInjure = _minialSegmentPool.Size();
            _potentialSegmentsToInjure = _subMinimalSegmentPool.Size();

        }

        public bool Disarmed()
        {

            return _requiredSegmentsToInjure == 0;
        }
}
    public enum SegmentType {
        KMinimal,
        KSubMinimal
    };

    public class CAxe
    {
        private CSegmentSquad _squad;
        private readonly CPoint[] _pointCage;
        private int _pointCageSize;
        private int _remainingForAxe;

        public CAxe(
            CSegmentSquad inSquad)
        {
            _squad = inSquad;
            _pointCage = new CPoint[3];
            _pointCageSize = 0;
            _remainingForAxe = 3;
        }


        public CPoint[] GetCage()
        {
            return _pointCage;
        }


        public bool CanAttack()
        {
            return _remainingForAxe > 0;
        }

        public bool Attack(
            CSegmentPool inSegmentPool)
        {
            bool wasSuccessful = false;

            CPoint theHuntedPoint = inSegmentPool.Injure();
            if (theHuntedPoint != null)
            {
                if( _remainingForAxe <= 0 )
                    throw new Exception("The axe can't attack anymore!");

                _remainingForAxe--;
                _pointCage[_pointCageSize++] = theHuntedPoint;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        
    }
}
