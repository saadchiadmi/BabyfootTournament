"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function getScore(team) {
    return team.players[0].goals + team.players[1].goals;
}
exports.getScore = getScore;
function newMatchTeam(team) {
    return { points: 0, players: [{ pseudo: team.players[0].pseudo, goals: 0 }, { pseudo: team.players[1].pseudo, goals: 0 }] };
}
exports.newMatchTeam = newMatchTeam;
//# sourceMappingURL=MatchTeam.js.map