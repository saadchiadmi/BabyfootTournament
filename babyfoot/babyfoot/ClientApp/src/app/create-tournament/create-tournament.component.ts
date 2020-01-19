import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../service/player.service';
import { Player } from '../entities/Player';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.component.html',
  styleUrls: ['./create-tournament.component.css']
})
export class CreateTournamentComponent implements OnInit {

  player: Player;
  p: Player;
  players: Player[]=[];
  filteredPlayersMultiple: Player[];

  constructor(private playerservice: PlayerService) { }

  ngOnInit() {
  }

  filterplayerMultiple(event) {
      let query = event.query;
      this.playerservice.getPlayers().subscribe(players => {
          this.filteredPlayersMultiple = this.filterplayer(query, players);
      });
  }

  filterplayer(query, players: Player[]):Player[] {
      //in a real application, make a request to a remote url with the query and return filtered results, for demo we filter at client side
      let filtered : Player[] = [];
      for(let i = 0; i < players.length; i++) {
          let player = players[i];
          if (player.pseudo.toLowerCase().indexOf(query.toLowerCase()) == 0) {
              filtered.push(player);
          }
      }
      return filtered;
  }

}
