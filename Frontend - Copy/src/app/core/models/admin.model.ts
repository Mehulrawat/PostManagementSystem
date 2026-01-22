


export enum AdminStatus {
    deleted=0,

  suspended = 1,
  promoted=2,
  demoted=3
  
  
}

export interface Users{
  id: string;
  userName: string,
  email:string
  isActive:boolean
  isAutoDeactivated:boolean
  createdAt:string,
  roles:string[]


}