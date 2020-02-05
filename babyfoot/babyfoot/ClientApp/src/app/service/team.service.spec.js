"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var team_service_1 = require("./team.service");
describe('TeamService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(team_service_1.TeamService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=team.service.spec.js.map