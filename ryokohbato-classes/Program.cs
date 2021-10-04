using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ryokohbato_classes
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // 土日の場合は何もしない
      if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
      {
        Environment.Exit(0);
      }

      // 講義開始時刻・講義開始5分前 のみ
      int period = -1;
      NotificationTime timing = NotificationTime.Start;

      // 1限開始5分前
      if (DateTime.Now.Hour == 8 && DateTime.Now.Minute == 40)
      {
        period = 1;
        timing = NotificationTime.Announce;
      }

      // 1限開始時刻
      if (DateTime.Now.Hour == 8 && DateTime.Now.Minute == 45)
      {
        period = 1;
        timing = NotificationTime.Start;
      }

      // 2限開始5分前
      if (DateTime.Now.Hour == 10 && DateTime.Now.Minute == 25)
      {
        period = 2;
        timing = NotificationTime.Announce;
      }

      // 2限開始時刻
      if (DateTime.Now.Hour == 10 && DateTime.Now.Minute == 30)
      {
        period = 2;
        timing = NotificationTime.Start;
      }

      // 3限開始5分前
      if (DateTime.Now.Hour == 13 && DateTime.Now.Minute == 10)
      {
        period = 3;
        timing = NotificationTime.Announce;
      }

      // 3限開始時刻
      if (DateTime.Now.Hour == 13 && DateTime.Now.Minute == 15)
      {
        period = 3;
        timing = NotificationTime.Start;
      }

      // 4限開始5分前
      if (DateTime.Now.Hour == 14 && DateTime.Now.Minute == 55)
      {
        period = 4;
        timing = NotificationTime.Announce;
      }

      // 4限開始時刻
      if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 0)
      {
        period = 4;
        timing = NotificationTime.Start;
      }

      // 5限開始5分前
      if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 40)
      {
        period = 5;
        timing = NotificationTime.Announce;
      }

      // 5限開始時刻
      if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 45)
      {
        period = 5;
        timing = NotificationTime.Start;
      }

      // 対象の時刻でなければ終了
      if (period < 0)
      {
        Environment.Exit(0);
      }

      string classesData;
      using(StreamReader sr = new StreamReader(Path.Combine(System.AppContext.BaseDirectory, "classes.json")))
      {
        classesData = sr.ReadToEnd();
      }

      var classesData__parsed = System.Text.Json.JsonDocument.Parse(classesData).RootElement.GetProperty(DateTime.Now.DayOfWeek.ToString());
      await Post2Slack(period, timing, classesData__parsed);
    }

    private enum NotificationTime
    {
      Announce,
      Start,
      Finish,
    }

    private static async Task Post2Slack(int period, NotificationTime timing, JsonElement oneDayclassesData)
    {
      Slack slack = new Slack();

      var classInfo = oneDayclassesData.GetProperty(period.ToString());

      if (classInfo.ToString().ToLower() == "false")
      {
        if (timing == NotificationTime.Announce)
        {
          await slack.PostMessageAsync("<@ryokohbato> 授業なし！", "ryokohbato-dev-log-zatsu", Keys.UserOAuthToken);
        }
      }
      else if (timing == NotificationTime.Announce)
      {
        await slack.PostMessageAsync(classInfo.GetProperty("Place") + "で" + classInfo.GetProperty("Title") + "があります。", "ryokohbato-dev-log-zatsu", Keys.UserOAuthToken);
      }
      else if (timing == NotificationTime.Start)
      {
        await slack.PostMessageAsync(period.ToString() + "限: " + classInfo.GetProperty("Title"), "ryokohbato-dev-log-zatsu", Keys.UserOAuthToken);
      }
    }
  }
}
