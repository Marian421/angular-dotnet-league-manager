export interface TeamCreate {
  name: string;
  isCompetitive: boolean;
  minAge: number;
  ownerId: number;
  memberIds?: number[];
}
