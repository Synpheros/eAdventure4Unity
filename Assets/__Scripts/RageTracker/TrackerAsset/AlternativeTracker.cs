﻿/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Collections;
using AssetPackage;

public class AlternativeTracker : TrackerAsset.IGameObjectTracker
{

    private TrackerAsset tracker;

    public void setTracker(TrackerAsset tracker)
    {
        this.tracker = tracker;
    }

    /* ALTERNATIVES */

    public enum Alternative
    {
        Question,
        Menu,
        Dialog,
        Path,
        Arena,
        Alternative
    }

    /// <summary>
    /// Player selected an option in a presented alternative
    /// Type = Alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    public void Selected(string alternativeId, string optionId)
    {
        tracker.Trace(new TrackerAsset.TrackerEvent()
        {
            Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Selected),
            Target = new TrackerAsset.TrackerEvent.TraceObject(Alternative.Alternative.ToString().ToLower(), alternativeId),
            Result = new TrackerAsset.TrackerEvent.TraceResult()
            {
                Response = optionId
            }
        });
    }

	/// <summary>
	/// Player selected an option in a presented alternative
	/// Type = Alternative
	/// </summary>
	/// <param name="alternativeId">Alternative identifier.</param>
	/// <param name="optionId">Option identifier.</param>
	/// <param name="correct">Is the selected alternative correct?.</param>
	public void Selected(string alternativeId, string optionId, bool correct)
	{
		tracker.Trace(new TrackerAsset.TrackerEvent()
			{
				Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Selected),
				Target = new TrackerAsset.TrackerEvent.TraceObject(Alternative.Alternative.ToString().ToLower(), alternativeId),
				Result = new TrackerAsset.TrackerEvent.TraceResult()
				{
					Response = optionId,
					Success = correct
				}
			});
	}

    /// <summary>
    /// Player selected an option in a presented alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    /// <param name="type">Alternative type.</param>
    public void Selected(string alternativeId, string optionId, Alternative type)
    {
        tracker.Trace(new TrackerAsset.TrackerEvent()
        {
            Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Selected),
            Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), alternativeId),
            Result = new TrackerAsset.TrackerEvent.TraceResult()
            {
                Response = optionId
            }
        });
    }

	/// <summary>
	/// Player selected an option in a presented alternative
	/// </summary>
	/// <param name="alternativeId">Alternative identifier.</param>
	/// <param name="optionId">Option identifier.</param>
	/// <param name="type">Alternative type.</param>
	/// <param name="correct">Is the selected alternative correct?.</param>
	public void Selected(string alternativeId, string optionId, bool correct, Alternative type)
	{
		tracker.Trace(new TrackerAsset.TrackerEvent()
			{
				Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Selected),
				Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), alternativeId),
				Result = new TrackerAsset.TrackerEvent.TraceResult()
				{
					Response = optionId,
					Success = correct
				}
			});
	}

    /// <summary>
    /// Player unlocked an option
    /// Type = Alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    public void Unlocked(string alternativeId, string optionId)
    {
        tracker.Trace(new TrackerAsset.TrackerEvent()
        {
            Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Unlocked),
            Target = new TrackerAsset.TrackerEvent.TraceObject(Alternative.Alternative.ToString().ToLower(), alternativeId),
            Result = new TrackerAsset.TrackerEvent.TraceResult()
            {
                Response = optionId
            }
        });
    }

    /// <summary>
    /// Player unlocked an option
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    /// <param name="type">Alternative type.</param>
    public void Unlocked(string alternativeId, string optionId, Alternative type)
    {
        tracker.Trace(new TrackerAsset.TrackerEvent()
        {
            Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Unlocked),
            Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), alternativeId),
            Result = new TrackerAsset.TrackerEvent.TraceResult()
            {
                Response = optionId
            }
        });
    }

}
