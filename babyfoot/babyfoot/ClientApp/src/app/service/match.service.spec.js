"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var match_service_1 = require("./match.service");
describe('MatchService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(match_service_1.MatchService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=match.service.spec.js.map