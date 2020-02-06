"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function getScore(team) {
    return team[0].goals + team[1].goals;
}
exports.getScore = getScore;
function newMatchTeam(team) {
    return [{ pseudo: team[0].pseudo, goals: 0 }, { pseudo: team[1].pseudo, goals: 0 }];
}
exports.newMatchTeam = newMatchTeam;
//# sourceMappingURL=MatchTeam.js.map