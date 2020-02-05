"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var player_service_1 = require("./player.service");
describe('PlayerService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(player_service_1.PlayerService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=player.service.spec.js.map