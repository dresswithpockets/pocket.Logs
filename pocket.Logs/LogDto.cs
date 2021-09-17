using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pocket.Logs
{
    public record LogDto(Dictionary<TeamName, TeamDto> Teams,
        int Length,
        Dictionary<string, PlayerDto> Players,
        Dictionary<string, string> Names,
        RoundDto[] Rounds,
        Dictionary<string, KeyValuePair<string, int>> HealSpread,
        Dictionary<string, Dictionary<string, int>> ClassKills,
        Dictionary<string, Dictionary<string, int>> ClassDeaths,
        Dictionary<string, Dictionary<string, int>> ClassKillAssists,
        ChatDto[] Chat,
        InfoDto Info,
        KillstreakDto[] Killstreaks);

    public enum TeamName
    {
        Red,
        Blue
    }

    public record TeamDto(int Score, int Kills, int Deaths, [property: JsonPropertyName("dmg")] int Damage, int Charges,
        int Drops, int FirstCaps, int Caps);

    public record PlayerDto(TeamName Team,
        [property: JsonPropertyName("class_stats")]
        ClassStatDto[] ClassStats,
        int Kills,
        int Deaths,
        int Assists,
        int Suicides,
        [property: JsonPropertyName("kapd")] int KillsAssistsPerDeath,
        [property: JsonPropertyName("kpd")] int KillsPerDeath,
        [property: JsonPropertyName("dmg")] int Damage,
        [property: JsonPropertyName("dmg_real")] int DamageReal,
        [property: JsonPropertyName("dt")] int DamageTaken,
        [property: JsonPropertyName("dt_real")] int DamageTakenReal,
        [property: JsonPropertyName("hr")] int HealsReceived,
        [property: JsonPropertyName("lks")] int Lks,
        [property: JsonPropertyName("as")] int Airshots,
        [property: JsonPropertyName("dapd")] int DamageAveragePerDeath,
        [property: JsonPropertyName("dapm")] int DamageAveragePerMinute,
        int Ubers,
        Dictionary<string, int> UberTypes,
        int Drops,
        int Medkits,
        [property: JsonPropertyName("medkits_hp")] int MedkitsHp,
        int Backstabs,
        int Headshots,
        [property: JsonPropertyName("headshots_hit")] int HeadshotsHit,
        int Sentries,
        [property: JsonPropertyName("heal")] int HealsGiven,
        [property: JsonPropertyName("cpc")] int Captures,
        [property: JsonPropertyName("ic")] int Ic,
        [property: JsonPropertyName("medicstats")] MedicStatsDto? MedicStats);

    public record ClassStatDto(string Type,
        int Kills,
        int Assists,
        int Deaths,
        [property: JsonPropertyName("dmg")] int Damage,
        [property: JsonPropertyName("weapon")] Dictionary<string, WeaponStatDto> WeaponStats,
        [property: JsonPropertyName("total_time")] int TotalTime);

    public record WeaponStatDto(int Kills,
        [property: JsonPropertyName("dmg")] int Damage,
        [property: JsonPropertyName("avg_dmg")] float AverageDamage,
        int Shots,
        int Hits);

    public record MedicStatsDto(
        [property: JsonPropertyName("advantages_lost")] int AdvantagesLost,
        [property: JsonPropertyName("biggest_advantage_lost")] int BiggestAdvantageLost,
        [property: JsonPropertyName("deaths_with_95_99_uber")] int DeathsWith95To99Uber,
        [property: JsonPropertyName("avg_time_before_healing")] float AverageTimeBeforeHealing,
        [property: JsonPropertyName("avg_time_to_build")] float AverageTimeToBuild,
        [property: JsonPropertyName("avg_time_before_using")] float AverageTimeBeforeUsing,
        [property: JsonPropertyName("avg_uber_length")] float AverageUberLength);

    public record RoundDto(
        [property: JsonPropertyName("start_time")] int StartTime,
        TeamName Winner,
        [property: JsonPropertyName("team")] Dictionary<TeamName, RoundTeamDto> Teams,
        Dictionary<string, string>[] Events,
        Dictionary<string, RoundPlayerDto> Players,
        TeamName FirstCap,
        int Length);

    public record RoundTeamDto(int Score, int Kills, [property: JsonPropertyName("dmg")] int Damage, int Ubers);
    
    public record RoundPlayerDto(TeamName Team, int Kills, [property: JsonPropertyName("dmg")] int Damage);

    public record ChatDto(
        string SteamId,
        string Name,
        [property: JsonPropertyName("msg")] string Message);

    public record InfoDto(
        string Map,
        bool Supplemental,
        [property: JsonPropertyName("total_length")] int TotalLength,
        bool HasRealDamage,
        bool HasWeaponDamage,
        bool HasAccuracy,
        bool HasHp,
        [property: JsonPropertyName("hasHP_real")] bool HasHpReal,
        bool HasHs,
        [property: JsonPropertyName("hasHS_hit")] bool HasHsHit,
        bool HasBs,
        bool HasCp,
        bool HasSb,
        bool HasDt,
        bool HasAs,
        bool HasHr,
        bool HasIntel,
        [property: JsonPropertyName("AD_scoring")] bool AdScoring,
        string Title,
        int Date,
        UploaderInfo Uploader);

    public record UploaderInfo(string Id, string Name, string Info);

    public record KillstreakDto();
}