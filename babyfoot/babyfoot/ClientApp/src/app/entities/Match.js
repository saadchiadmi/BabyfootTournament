"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var MatchTeam_1 = require("./MatchTeam");
function getMatch(matches, stage, order) {
    return matches.filter(function (m) { return m.stage === stage && m.order == order; })[0];
}
exports.getMatch = getMatch;
function getFinalMatch(matches) {
    return matches.filter(function (m) { return m.stage === "Final"; })[0];
}
exports.getFinalMatch = getFinalMatch;
function getSemifinalMatch(matches, order) {
    return getMatch(matches, "Semifinal", order);
}
exports.getSemifinalMatch = getSemifinalMatch;
function getPoolMatch(matches, order) {
    return getMatch(matches, "Pool", order);
}
exports.getPoolMatch = getPoolMatch;
function getWinnerTeam(match) {
    var score1 = MatchTeam_1.getScore(match.teams[0]);
    var score2 = MatchTeam_1.getScore(match.teams[1]);
    if (score1 > score2)
        return match.teams[0];
    else if (score1 < score2)
        return match.teams[1];
    else
        return null;
}
exports.getWinnerTeam = getWinnerTeam;
//# sourceMappingURL=Match.js.map