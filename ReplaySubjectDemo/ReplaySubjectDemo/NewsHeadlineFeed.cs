using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//https://msdn.microsoft.com/en-us/library/hh211810(v=vs.103).aspx
namespace ReplaySubjectDemo
{
	public class NewsHeadlineFeed
	{
		private string feedName;                     // Feedname used to label the stream
		private IObservable<string> headlineFeed;    // The actual data stream
		private readonly Random rand = new Random(); // Used to stream random headlines.

		//*** A list of predefined news events to combine with a simple location string ***//
		static readonly string[] newsEvents = { "A tornado occurred ",
											"Weather watch for snow storm issued ",
											"A robbery occurred ",
											"We have a lottery winner ",
											"An earthquake occurred ",
											"Severe automobile accident "};

		//*** A list of predefined location strings to combine with a news event. ***//
		static readonly string[] newsLocations = { "in your area.",
											   "in Dallas, Texas.",
											   "somewhere in Iraq.",
											   "Lincolnton, North Carolina",
											   "Redmond, Washington"};

		public IObservable<string> HeadlineFeed
		{
			get { return headlineFeed; }
		}

		//****************************************************************//
		//*** Some very simple formatting of the headline event string ***//
		//****************************************************************//
		private string RandNewsEvent()
		{
			return "Feedname     : " + feedName + "\nHeadline     : " + newsEvents[rand.Next(newsEvents.Length)] +
				   newsLocations[rand.Next(newsLocations.Length)];
		}

		public NewsHeadlineFeed(string name)
		{
			feedName = name;

			//*****************************************************************************************//
			//*** Using the Generate operator to generate a continous stream of headline that occur ***//
			//*** randomly within 5 seconds.                                                        ***//
			//*****************************************************************************************//
			headlineFeed = Observable.Generate(RandNewsEvent(),
											   evt => true,
											   evt => RandNewsEvent(),
											   evt => { Thread.Sleep(rand.Next(3000)); return evt; },
											   Scheduler.Default);
		}
	}
}
