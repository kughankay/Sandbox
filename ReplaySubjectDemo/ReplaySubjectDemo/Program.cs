using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReplaySubjectDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			ReplaySubject<Timestamped<string>> myReplaySubject = new ReplaySubject<Timestamped<String>>();

			//*****************************************************************//
			//*** Create news feed #1 and subscribe the ReplaySubject to it ***//
			//*****************************************************************//

			NewsHeadlineFeed NewsFeed1 = new NewsHeadlineFeed("Headline News Feed #1");
			NewsFeed1.HeadlineFeed.Timestamp().Subscribe(myReplaySubject);

			//*****************************************************************//
			//*** Create news feed #2 and subscribe the ReplaySubject to it ***//
			//*****************************************************************//

			NewsHeadlineFeed NewsFeed2 = new NewsHeadlineFeed("Headline News Feed #2");
			NewsFeed2.HeadlineFeed.Timestamp().Subscribe(myReplaySubject);


			//*****************************************************************************************************//
			//*** Create a subscription to the subject's observable sequence. This subscription will filter for ***//
			//*** only local headlines that occurred 10 seconds before the subscription was created.            ***//
			//***                                                                                               ***//
			//*** Since we are using a ReplaySubject with timestamped headlines, we can subscribe to the        ***//
			//*** headlines already past. The ReplaySubject will "replay" them for the localNewSubscription     ***//
			//*** from its buffered sequence of headlines.                                                      ***//
			//*****************************************************************************************************//

			Console.WriteLine("Waiting for 10 seconds before subscribing to local news headline feed.\n");
			Thread.Sleep(10000);

			Console.WriteLine("\n*** Creating local news headline subscription at {0} ***\n", DateTime.Now.ToString());
			Console.WriteLine("This subscription asks the ReplaySubject for the buffered headlines that\n" +
							  "occurred within the last 10 seconds.\n\nPress ENTER to exit.", DateTime.Now.ToString());


			DateTime lastestHeadlineTime = DateTime.Now;
			DateTime earliestHeadlineTime = lastestHeadlineTime - TimeSpan.FromSeconds(10);

			IDisposable localNewsSubscription = myReplaySubject.Where(x => x.Value.Contains("in your area.") &&
																	 (x.Timestamp >= earliestHeadlineTime) &&
																	 (x.Timestamp < lastestHeadlineTime)).Subscribe(x =>
																	 {
																		 Console.WriteLine("\n************************************\n" +
																					 "***[ Local news headline report ]***\n" +
																					 "************************************\n" +
																					 "Time         : {0}\n{1}\n\n", x.Timestamp.ToString(), x.Value);
																	 });

			Console.ReadLine();


			//*******************************//
			//*** Cancel the subscription ***//
			//*******************************//

			localNewsSubscription.Dispose();


			//*************************************************************************//
			//*** Unsubscribe all the ReplaySubject's observers and free resources. ***//
			//*************************************************************************//

			myReplaySubject.Dispose();
		}
	}
}
