"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var tournament_service_1 = require("./tournament.service");
describe('TournamentService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(tournament_service_1.TournamentService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=tournament.service.spec.js.map