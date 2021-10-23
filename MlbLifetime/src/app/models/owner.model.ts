export type YearType = 'Regular' | 'Post';
export type PositionType = 'Pitcher' | 'Batter';

export interface ILeagueData {
    Year: number;
    SeasonType: YearType;
    LastUpdated: string;
    ProbablesDate: string;
    Owners: Owner[];
}


export interface IOwner {
    Name: string;
    BatterScore: number;
    PitcherScore: number;
    Batters: IBatter[];
    Pitchers: IPitcher[];
}

export interface IPlayer {
    FantasyOwner: string;
    Name: string;
    Link: string;
    Position: Position;
    PositionType: PositionType;
    GamesPlayed: number;
    Age: number;
    Total: number;
}

export interface IBatter extends IPlayer {
    AB: number;
    H: number;
    R: number;
    HR: number;
    RBI: number;
    SB: number;
    BB: number;
}

export interface IPitcher extends IPlayer {
    W: number;
    IP: number;
    SO: number;
    SV: number;
    Probable: boolean;
}

export class LeagueData implements ILeagueData {
    Year!: number;
    SeasonType!: YearType;
    LastUpdated!: string;
    ProbablesDate!: string;
    Owners!: Owner[];
}

export class Owner implements IOwner {
    Name!: string;
    BatterScore!: number;
    PitcherScore!: number;
    Batters!: Batter[];
    Pitchers!: Pitcher[];
    CountingPlayers!: number;
}

export class Batter implements IBatter {
    AB!: number;
    H!: number;
    R!: number;
    HR!: number;
    RBI!: number;
    SB!: number;
    BB!: number;
    FantasyOwner!: string;
    Name!: string;
    Link!: string;
    Position!: Position;
    PositionType!: PositionType;
    GamesPlayed!: number;
    Age!: number;

    Total!: number;
}

export class Pitcher implements IPitcher {
    W!: number;
    IP!: number;
    SO!: number;
    SV!: number;
    FantasyOwner!: string;
    Name!: string;
    Link!: string;
    Position!: Position;
    PositionType!: PositionType;
    GamesPlayed!: number;
    Age!: number;
    Probable!: boolean;

    Total!: number;
}

export interface ITotalView {
    FantasyOwner: string;
    Count: number;
    IndexTotal: number;
}

export enum Position {
    None,
    Batter,
    Pitcher
}


