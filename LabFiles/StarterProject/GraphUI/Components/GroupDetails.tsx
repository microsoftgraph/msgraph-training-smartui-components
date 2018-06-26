import * as React from 'react';
import * as ReactDOM from 'react-dom';

import { Group, IGroupListState, DriveItem } from './GroupList';

export interface IGroupDetailsProps {
  group: Group
}

export class GroupDetails extends React.Component<IGroupDetailsProps, any> {
  userId: string;

  constructor(props: IGroupDetailsProps) {
    super(props);
  }

  public render() {
    const group = this.props.group;
    return (
      <div>
        <h2>{this.props.group.name}</h2>
      </div>
    );
  }
}