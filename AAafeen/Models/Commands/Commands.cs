using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

/*
 * --記載ルール--
 * ・メンバ名はすべて小文字
 * ・Commadsクラスのメンバ（つまりコマンドで使えるメソッド）の引数は一つだけでstringを返す
 * ・～Argsのプロパティの型はすべてstring
 */

namespace AAafeen.Models.Commands
{
    /// <summary>
    /// コマンドで使えるメソッド
    /// </summary>
    public static class Methods
    {
        public static TwitterContext twCtx = new TwitterContext();

        private static string CreateStatusText(Status source)
        {
            return string.Format(
                "ID:{0}\nUser:{1} / {2}\nText:{3}\n{4}CreatedAt:{5}\nSource:{6}",
                source.StatusID,
                source.User.Identifier.ScreenName,
                source.User.Name,
                source.Text,
                string.IsNullOrEmpty(source.InReplyToStatusID) ?
                    "" : string.Format("InReplyToId:{0}\n", source.InReplyToStatusID),
                source.CreatedAt.ToLocalTime(),
                source.Source
            );
        }

        private static string CreateStatusesText(IEnumerable<Status> source)
        {
            return string.Join("\n\n", source.Select(CreateStatusText));
        }

        public static string posttweet(PostTweetArgs args)
        {
            return CreateStatusText(twCtx.UpdateStatus(args.text, args.inreplytoid));
        }

        public static string getpublictimeline(object paramNothing)
        {
            return CreateStatusesText(twCtx.Status.Where(tweet => tweet.Type == StatusType.Public));
        }

        public static string getusertimeline(GetUserTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.User &&
                    tweet.ID == args.id &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }

        public static string gethometimeline(GetTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.Home &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }

        public static string getmentions(GetTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.Mentions &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }

        public static string showtweet(RequestIdArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.Show &&
                    tweet.ID == args.id
                )
            );
        }

        private static string CreateUserText(User source)
        {
            if (source.Status != null)
                source.Status.User = source;
            return string.Format(@"ID:{0}
Name:{1} / {2}
Location:{3}
Description:{4}
CreatedAt:{5}
WebSite:{6}
Statuses:{7}
Friends:{8}
Followers:{9}
FavCount:{10}
{11}{12}{13}",
                source.Identifier.UserID,
                source.Identifier.ScreenName,
                source.Name,
                source.Location,
                source.Description,
                source.CreatedAt.ToLocalTime(),
                source.URL,
                source.StatusesCount,
                source.FriendsCount,
                source.FollowersCount,
                source.FavoritesCount,
                source.Verified ? "Verified" : "",
                source.Protected ? "Protected" : "",
                source.Status == null ? "" :
                string.Format("LastTweet\n\t{0}",
                    string.Join("\n\t", CreateStatusText(source.Status).Split('\n'))
                )
            );
        }

        private static string CreateUsersText(IEnumerable<User> source)
        {
            var _source = source.ToArray();
            string cursors = "";
            var first = _source.FirstOrDefault();
            if (first != null && !string.IsNullOrEmpty(first.CursorMovement.Next))
                cursors = string.Format("\n\nNextCursor:{0}\nPreviousCursor:{1}",
                    first.CursorMovement.Next, first.CursorMovement.Previous);
            return string.Join("\n\n", _source.Select(CreateUserText)) + cursors;
        }

        public static string showuser(RequestIdArgs args)
        {
            return CreateUsersText(
                twCtx.User.Where(user =>
                    user.Type == UserType.Show &&
                    user.ID == args.id
                )
            );
        }

        //401を返してくるので保留
        public static string lookupusers(RequestIdArgs args)
        {
            return CreateUsersText(
                twCtx.User.Where(user =>
                    user.Type == UserType.Lookup &&
                    user.ScreenName == args.id
                )
            );
        }

        public static string getfriends(RequestIdAndCursorArgs args)
        {
            return CreateUsersText(
                twCtx.User.Where(user =>
                    user.Type == UserType.Friends &&
                    user.ID == args.id &&
                    user.Cursor == args.cursor
                )
            );
        }

        public static string getfollowers(RequestIdAndCursorArgs args)
        {
           return CreateUsersText(
               twCtx.User.Where(user =>
                   user.Type == UserType.Followers &&
                   user.ID == args.id &&
                   user.Cursor == args.cursor
               )
           );
        }

        private static string CreateIdsText(SocialGraph source)
        {
            return string.Format(
                "{0}\n\nNextCursor:{1}\nPreviousCursor:{2}",
                string.Join("\n", source.IDs),
                source.CursorMovement.Next,
                source.CursorMovement.Previous
            );
        }

        public static string getfriendids(RequestIdAndCursorArgs args)
        {
            return CreateIdsText(
                twCtx.SocialGraph.Where(_ =>
                    _.Type == SocialGraphType.Friends &&
                    _.ID == args.id &&
                    _.Cursor == args.cursor
                )
                .First()
            );
        }

        public static string getfollowerids(RequestIdAndCursorArgs args)
        {
            return CreateIdsText(
                twCtx.SocialGraph.Where(_ =>
                    _.Type == SocialGraphType.Followers &&
                    _.ID == args.id &&
                    _.Cursor == args.cursor
                )
                .First()
            );
        }

        public static string getretweetedbyme(GetTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.RetweetedByMe &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }

        public static string getretweetedtome(GetTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.RetweetedToMe &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }

        public static string getretweetsofme(GetTimelineArgs args)
        {
            return CreateStatusesText(
                twCtx.Status.Where(tweet =>
                    tweet.Type == StatusType.RetweetsOfMe &&
                    tweet.Count == int.Parse(args.count) &&
                    tweet.Page == int.Parse(args.page)
                )
            );
        }
    }

    //以下コマンドの引数になる型
    public class PostTweetArgs
    {
        public string inreplytoid { set; get; }
        public string text { set; get; }
    }
    
    public class GetTimelineArgs
    {
        public GetTimelineArgs()
        {
            count = "20";
            page = "1";
        }

        public string count { set; get; }
        public string page { set; get; }
    }

    public class GetUserTimelineArgs : GetTimelineArgs
    {
        public string id { set; get; }
    }

    public class RequestIdArgs
    {
        public string id { set; get; }
    }

    public class RequestIdAndCursorArgs : RequestIdArgs
    {
        public RequestIdAndCursorArgs()
        {
            cursor = "-1";
        }

        public string cursor { set; get; }
    }

    public class GetListArgs : RequestIdAndCursorArgs
    {
        public string listname { set; get; }
    }
}
