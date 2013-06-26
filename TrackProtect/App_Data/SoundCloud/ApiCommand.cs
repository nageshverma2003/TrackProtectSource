using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// The flow is initiated by directing the end user in a browser to the SoundCloud authorization endpoint which is
    /// </summary>
    public enum ApiCommand
    {
        AuthorizationCodeFlow,
        UserAgentFlow,
        UserCredentialsFlow,
        RefreshToken,
        Me,
        MeTracks,
        MeComments,
        MeFollowings,
        MeFollowingsContact,
        MeFollowers,
        MeFollowersContact,
        MeFavorites,
        MeFavoritesTrack,
        MeGroups,
        MePlaylists,
        MeConnections,
        Users,
        User,
        UserTracks,
        UserComments,
        UserFollowings,
        UserFollowingsContact,
        UserFollowers,
        UserFollowersContact,
        UserFavorites,
        UserFavoritesTrack,
        UserGroups,
        UserPlaylists,
        Tracks,
        Track,
        TrackComments,
        TrackPermissions,
        TrackSecretToken,
        TrackShare,
        Comment,
        Groups,
        Group,
        GroupUsers,
        GroupModerators,
        GroupMembers,
        GroupContributors,
        GroupTracks,
        Playlists,
        Playlist,
        Resolve
    }
}