namespace TrackProtect.Facebook
{
    /*
       "id": "100001465318209",
       "name": "Lambert van Lieshout",
       "first_name": "Lambert",
       "middle_name": "van",
       "last_name": "Lieshout",
       "link": "http://www.facebook.com/lambert.vanlieshout",
       "username": "lambert.vanlieshout",
       "gender": "male",
       "timezone": 2,
       "locale": "en_US",
       "verified": true,
       "updated_time": "2012-09-23T17:06:49+0000"
    */

    /// <summary>
    /// Result of a Facebook Me request. Not complete, only needed properties.
    /// </summary>
    public class Me
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }

    public class MeData
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }
}