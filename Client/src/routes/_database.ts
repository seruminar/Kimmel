import Dexie from 'dexie';

export interface IKml {
  id?: number;
  kml: string;
  name: string;
}

export class KmlDatabase extends Dexie {
  public kmls: Dexie.Table<IKml, number>;

  public constructor() {
    super("KmlDatabase");
    this.version(1).stores({
      kmls: "++id,kml,name",
    });

    this.kmls = this.table("kmls");
  }
}
