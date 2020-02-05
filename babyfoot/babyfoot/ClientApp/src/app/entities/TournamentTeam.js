"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function getTeam(mteam, teams) {
    var pseudos = mteam.players.map(function (p) { return p.pseudo; });
    var team = teams.filter(function (t) { return t.players.map(function (p) { return p.pseudo; }).some(function (pseudo) { return pseudos.some(function (p) { return p === pseudo; }); }); })[0];
    return team;
}
exports.getTeam = getTeam;
function toMatchTeam(team) {
    return {
        players: [
            { pseudo: team.players[0].pseudo, goals: 0 },
            { pseudo: team.players[1].pseudo, goals: 0 }
        ],
        points: 0
    };
}
exports.toMatchTeam = toMatchTeam;
//# sourceMappingURL=Team.js.map