using System.Linq;
using Content.Server.Administration.Systems;
using Content.Server.Antag;
using Content.Server.Antag.Components;
using Content.Server._Nineveh.Antag.Components;
using Content.Server._Nineveh.GameTicking.Components;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Spawners.Components;
using Content.Server.Station.Components;
using Content.Shared.EntityTable;
using Content.Shared.GameTicking;
using Content.Shared.GameTicking.Components;
using Content.Shared.Preferences;
using Content.Shared.Players;
using Content.Shared.Roles;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Nineveh.Antag;

public sealed class NinevehAntagSelectionSystem : EntitySystem
{
    [Dependency] private readonly AdminSystem _admin = default!;
    [Dependency] private readonly AntagSelectionSystem _antagSelection = default!;
    [Dependency] private readonly EntityTableSystem _entityTable = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private static readonly ProtoId<JobPrototype> PassengerJob = "Passenger";

    public bool TryRunNinevehRoundStart(EntityUid uid,
        NinevehRollingGameRuleComponent rolling,
        AntagSelectionComponent antagSelection,
        List<ICommonSession> playerPool,
        IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> profiles)
    {
        if (!TryGetPrimaryStation(out var station))
        {
            Log.Error("Nineveh rolling failed to find a spawnable station.");
            return false;
        }

        var pairSessions = new List<ICommonSession>();
        if (TrySelectSpawner(rolling, playerPool.Count, out var selectedSpawner) &&
            selectedSpawner != null &&
            TrySpawnPair((uid, antagSelection), station, selectedSpawner.Value, playerPool, profiles, pairSessions))
        {
            rolling.SelectedPairSpawner = selectedSpawner.Value;
            rolling.PairSelectionComplete = true;
        }
        else
        {
            Log.Warning("Nineveh rolling could not produce a valid antag pair. Falling back to civilians-only spawn flow.");
        }

        foreach (var session in pairSessions)
        {
            playerPool.Remove(session);
        }

        foreach (var session in playerPool.ToArray())
        {
            if (!profiles.TryGetValue(session.UserId, out var profile))
                continue;

            SpawnCivilian(session, profile, station);
            playerPool.Remove(session);
        }

        rolling.CivilianHandoffComplete = true;
        return true;
    }

    private bool TrySelectSpawner(NinevehRollingGameRuleComponent rolling, int readyPlayerCount, out EntProtoId? spawnerId)
    {
        spawnerId = null;

        if (!_prototype.TryIndex(rolling.PairTable, out var table))
        {
            Log.Error($"Nineveh rolling is missing pair table '{rolling.PairTable}'.");
            return false;
        }

        var seed = HashCode.Combine(_gameTicker.RoundId, readyPlayerCount, rolling.PairTable.Id);
        var rand = new Random(seed ^ _random.Next());
        spawnerId = _entityTable.GetSpawns(table, rand).FirstOrDefault();
        return spawnerId != null;
    }

    private bool TrySpawnPair(Entity<AntagSelectionComponent> rule,
        EntityUid station,
        EntProtoId spawnerId,
        List<ICommonSession> playerPool,
        IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> profiles,
        List<ICommonSession> pairSessions)
    {
        var spawner = Spawn(spawnerId, MapCoordinates.Nullspace);
        if (!TryComp<NinevehAntagPairSpawnerComponent>(spawner, out var pair))
        {
            Log.Error($"Nineveh pair spawner '{spawnerId}' is missing {nameof(NinevehAntagPairSpawnerComponent)}.");
            QueueDel(spawner);
            return false;
        }

        if (!TryPickSession(rule, pair.PrimaryDefinition, playerPool, out var primarySession) ||
            !TryPickSession(rule, pair.SecondaryDefinition, playerPool, out var secondarySession))
        {
            QueueDel(spawner);
            return false;
        }

        if (!profiles.TryGetValue(primarySession.UserId, out var primaryProfile) ||
            !profiles.TryGetValue(secondarySession.UserId, out var secondaryProfile))
        {
            QueueDel(spawner);
            return false;
        }

        var primaryCoords = GetSpawnCoordinates(pair.PrimaryMarker, pair.PrimaryFallbackJob, station);
        var secondaryCoords = GetSpawnCoordinates(pair.SecondaryMarker, pair.SecondaryFallbackJob, station);
        if (primaryCoords == null || secondaryCoords == null)
        {
            Log.Error($"Nineveh pair '{pair.PairId}' could not find valid spawn coordinates.");
            QueueDel(spawner);
            return false;
        }

        var primaryMob = SpawnAssignedMob(primarySession, primaryProfile, station, pair.PrimaryMob, primaryCoords.Value, canBeAntag: true);
        var secondaryMob = SpawnAssignedMob(secondarySession, secondaryProfile, station, pair.SecondaryMob, secondaryCoords.Value, canBeAntag: true);
        if (primaryMob == null || secondaryMob == null)
        {
            QueueDel(spawner);
            return false;
        }

        _antagSelection.MakeAntag(rule, primarySession, pair.PrimaryDefinition, ignoreSpawner: true);
        _antagSelection.MakeAntag(rule, secondarySession, pair.SecondaryDefinition, ignoreSpawner: true);

        pairSessions.Add(primarySession);
        pairSessions.Add(secondarySession);

        QueueDel(spawner);
        return true;
    }

    private bool TryPickSession(Entity<AntagSelectionComponent> rule,
        AntagSelectionDefinition definition,
        List<ICommonSession> playerPool,
        out ICommonSession session)
    {
        session = default!;

        var pool = _antagSelection.GetPlayerPool(rule, playerPool, definition);
        if (!_antagSelection.TryPickAntagSession(pool.List, definition.PrefRoles, out var picked) || picked == null)
            return false;

        session = picked;
        playerPool.Remove(session);
        return true;
    }

    private void SpawnCivilian(ICommonSession session, HumanoidCharacterProfile profile, EntityUid station)
    {
        _gameTicker.DoSpawn(session, profile, station, PassengerJob, false, out var mob, out _, out _, SpawnPointType.Job);
        RaiseSpawnComplete(mob, session, PassengerJob, station, profile, canBeAntag: false);
    }

    private EntityUid? SpawnAssignedMob(ICommonSession session,
        HumanoidCharacterProfile profile,
        EntityUid station,
        EntProtoId mobPrototype,
        EntityCoordinates coordinates,
        bool canBeAntag)
    {
        _gameTicker.PlayerJoinGame(session);

        var existingMind = session.GetMind();
        EntityUid mind;
        if (existingMind != null)
        {
            mind = existingMind.Value;
        }
        else
        {
            mind = _mind.CreateMind(session.UserId, profile.Name);
            _mind.SetUserId(mind, session.UserId);
        }

        var mob = Spawn(mobPrototype, coordinates);
        _mind.TransferTo(mind, mob, ghostCheckOverride: true);
        _admin.UpdatePlayerList(session);
        RaiseSpawnComplete(mob, session, null, station, profile, canBeAntag);
        return mob;
    }

    private void RaiseSpawnComplete(EntityUid mob,
        ICommonSession session,
        string? jobId,
        EntityUid station,
        HumanoidCharacterProfile profile,
        bool canBeAntag)
    {
        _gameTicker.PlayersJoinedRoundNormally++;
        var ev = new PlayerSpawnCompleteEvent(mob,
            session,
            jobId,
            false,
            false,
            _gameTicker.PlayersJoinedRoundNormally,
            station,
            profile,
            canBeAntag);
        RaiseLocalEvent(mob, ev, true);
    }

    private EntityCoordinates? GetSpawnCoordinates(EntProtoId? markerProto, ProtoId<JobPrototype>? fallbackJob, EntityUid station)
    {
        var stationMap = Transform(station).MapUid;
        if (stationMap == null)
            return null;

        var markerPositions = new List<EntityCoordinates>();
        var markerQuery = EntityQueryEnumerator<SpawnPointComponent, TransformComponent, MetaDataComponent>();
        while (markerQuery.MoveNext(out var uid, out _, out var xform, out var meta))
        {
            if (TerminatingOrDeleted(uid) || xform.MapUid == null || xform.MapUid != stationMap || meta.EntityPrototype?.ID != markerProto)
                continue;

            markerPositions.Add(xform.Coordinates);
        }

        if (markerPositions.Count > 0)
            return _random.Pick(markerPositions);

        var fallbackPositions = new List<EntityCoordinates>();
        var spawnQuery = EntityQueryEnumerator<SpawnPointComponent, TransformComponent>();
        while (spawnQuery.MoveNext(out var uid, out var spawnPoint, out var xform))
        {
            if (TerminatingOrDeleted(uid) || xform.MapUid == null || xform.MapUid != stationMap)
                continue;

            if (spawnPoint.SpawnType != SpawnPointType.Job)
                continue;

            if (fallbackJob != null && spawnPoint.Job != null && spawnPoint.Job != fallbackJob)
                continue;

            fallbackPositions.Add(xform.Coordinates);
        }

        if (fallbackPositions.Count > 0)
            return _random.Pick(fallbackPositions);

        return null;
    }

    private bool TryGetPrimaryStation(out EntityUid station)
    {
        var query = EntityQueryEnumerator<StationJobsComponent, StationSpawningComponent>();
        while (query.MoveNext(out var uid, out _, out _))
        {
            station = uid;
            return true;
        }

        station = EntityUid.Invalid;
        return false;
    }
}
