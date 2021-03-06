using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GammaDraconis.Types;

namespace GammaDraconis.Core
{
    /// <summary>
    /// Manages a race, given a set of Racers and a Course.
    /// </summary>
    class Race
    {
        // The course for the race
        private Course course;
        private int laps;

        private bool RaceOver = false;
        public bool isRaceOver()
        {
            return RaceOver;
        }

        /// <summary>
        /// Keep track of when this race began.
        /// </summary>
        private double startTime;
        public double StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        // The racers and their statuses
        private Dictionary<Racer, int> state;
        private List<Racer> finishedRacers;


        /// <summary>
        /// Create a race manager.
        /// </summary>
        /// <param name="course">The race course</param>
        /// <param name="racers">The racers</param>
        public Race(Course course, Racer[] racers)
        {
            this.course = course;
            this.laps = (course.loop ? course.laps : 1);
            state = new Dictionary<Racer, int>();
            foreach (Racer racer in racers)
            {
                state.Add(racer, 0);
            }
            finishedRacers = new List<Racer>();
        }

        /// <summary>
        /// Get an upcoming coordinate for the given racer.
        /// </summary>
        /// <param name="racer">The racer object</param>
        /// <param name="offset">How many coordinates forward in the race</param>
        /// <returns>Coordinate object, or Null if past end of race</returns>
        public Checkpoint checkpoint(Racer racer, int offset)
        {
            RaceStatus info = status(racer, true);
            int lap = info.lap;
            int point = info.checkpoint + offset;

            while (point > course.path.Count)
            {           
                if (course.loop)
                {
                    if (lap >= laps && point == course.checkpoints.Count + 1)
                    {
                        return course.checkpoints[0];
                    }
                }
                else
                {
                    // past end of course
                    return null;
                }
                lap++;
                point -= course.path.Count;
            }
            if (point == 0)
            {
                point = course.path.Count;
            }

            return course.checkpoints[point - 1].clone();
        }

        /// <summary>
        /// Get the next checkpoint a racer must reach.
        /// </summary>
        /// <param name="racer"></param>
        /// <returns></returns>
        public Checkpoint nextCheckpoint(Racer racer) { 
            return checkpoint(racer, 1); 
        }

        /// <summary>
        /// Returns the number of checkpoints in this course.
        /// </summary>
        /// <returns></returns>
        public int length()
        {
            return course.checkpoints.Count;
        }

        /// <summary>
        /// See if there is a winner yet.
        /// </summary>
        /// <returns>Null if no winner, Racer object otherwise</returns>
        public Racer winner()
        {
            if (finishedRacers.Count > 0)
            {
                return finishedRacers[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an ordered list of finished racers.
        /// </summary>
        /// <returns></returns>
        public List<Racer> rankings()
        {
            return new List<Racer>(finishedRacers);
        }

        /// <summary>
        /// Update the current status of the Race manager.
        /// </summary>
        public void update(GameTime gameTime)
        {
            foreach (Racer r in new List<Racer>(state.Keys))
            {
                if (!finishedRacers.Contains(r))
                {
                    BoundingSphere checkpointSphere = new BoundingSphere(nextCheckpoint(r).position.pos(), nextCheckpoint(r).size);
                    BoundingSphere racerSphere = new BoundingSphere(r.position.pos(), r.size);
                    if (checkpointSphere.Intersects(racerSphere))
                    {
                        //Racer has passed a checkpoint
                        state[r] += 1;
                        Audio.play("pass_checkpoint");

                        if (state[r] == laps * course.checkpoints.Count + (course.loop ? 0 : -1))
                        {
                            finishedRacers.Add(r);
                            r.time = gameTime.TotalRealTime.TotalMilliseconds - startTime;
                            if (finishedRacers.Count == state.Count)
                            {
                                RaceOver = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get a the lap and checkpoint 
        /// </summary>
        /// <param name="racer"></param>
        /// <returns></returns>
        public RaceStatus status(Racer racer)
        {
            return status(racer, false);
        }

        /// <summary>
        /// Get a the lap and checkpoint 
        /// </summary>
        /// <param name="racer"></param>
        /// <param name="minimal"></param>
        /// <returns></returns>
        public RaceStatus status(Racer racer, bool minimal)
        {
            int status = state[racer];
            RaceStatus raceStatus = new RaceStatus();
            raceStatus.lap = status / course.path.Count + 1;
            raceStatus.checkpoint = status % course.path.Count + 1;
            if (!minimal)
            {
                raceStatus.place = finishedRacers.IndexOf(racer) + 1;
                raceStatus.leading = 0;
                raceStatus.following = 0;
                foreach (Racer r in new List<Racer>(state.Keys))
                {
                    if( r != racer )
                    {
                        if (state[r] < status)
                        {
                            raceStatus.leading++;
                        }
                        else if (state[r] > status)
                        {
                            raceStatus.following++;
                        }
                    }
                }
            }
            return raceStatus;
        }
    }

    /// <summary>
    /// Maintains all of the status variables for the race
    /// </summary>
    public struct RaceStatus
    {
        public int lap;
        public int checkpoint;
        public int place;
        public int leading;
        public int following;
    }
}
