import { Component, OnInit } from '@angular/core';
import { Player } from '../entities/Player';
import { SelectItem } from 'primeng/primeng';
import { PlayerService } from '../service/player.service';


@Component({
    selector: 'app-players',
    templateUrl: './players.component.html',
    styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {

    players: Player[];
    selectedPlayer: Player;
    displayDialog: boolean;
    pseudo: string = "";
    sortOptions: SelectItem[];
    sortKey: string;
    sortField: string;
    sortOrder: number;

    constructor(private playerservice: PlayerService) { }

    ngOnInit() {
        this.playerservice.getPlayers().subscribe(players => this.players = players);

        this.sortOptions = [
            { label: 'Champions', value: '!champions' },
            { label: 'Goals', value: '!goals' },
            { label: 'Score', value: '!score' }
        ];
    }

    selectPlayer(event: Event, player: Player) {
        this.selectedPlayer = player;
        this.displayDialog = true;
        event.preventDefault();
    }

    onSortChange(event) {
        let value = event.value;

        if (value.indexOf('!') === 0) {
            this.sortOrder = -1;
            this.sortField = value.substring(1, value.length);
        }
        else {
            this.sortOrder = 1;
            this.sortField = value;
        }
    }

    onDialogHide() {
        this.selectedPlayer = null;
    }

    search(pseudo) {
        //this.pseudo = pseudo;
        this.playerservice.getPlayers().subscribe(players => this.players = players.filter(p => p.pseudo.match(pseudo)));
    }

    addPlayer() {
        this.playerservice.savePlayer({ pseudo: this.pseudo, champions: 0, goals: 0, score: 0 })
            .subscribe(res => {
                this.pseudo = "";
                this.search(this.pseudo);
            });

    }
}