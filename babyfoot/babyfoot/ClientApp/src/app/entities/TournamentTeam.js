"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function getTeam(mteam, teams) {
    var pseudos = mteam.players.map(function (p) { return p.pseudo; });
    var team = teams.filter(function (t) { return t.pseudos.some(function (pseudo) { return pseudos.some(function (p) { return p === pseudo; }); }); })[0];
    return team;
}
exports.getTeam = getTeam;
function toMatchTeam(team) {
    return {
        players: [
            { pseudo: team.pseudos[0], goals: 0 },
            { pseudo: team.pseudos[1], goals: 0 }
        ],
        points: 0
    };
}
exports.toMatchTeam = toMatchTeam;
//# sourceMappingURL=TournamentTeam.js.map