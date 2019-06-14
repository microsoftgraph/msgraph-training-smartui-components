import * as React from 'react';
import * as ReactDOM from 'react-dom';
import {
  IObjectWithKey,
  ISelection,
  Selection,
  SelectionMode,
  SelectionZone
} from 'office-ui-fabric-react/lib/Selection';
import {
  FocusZone,
  FocusZoneDirection
} from 'office-ui-fabric-react/lib/FocusZone';
import {
  Image,
  ImageFit
} from 'office-ui-fabric-react/lib/Image';
import {
  Panel,
  PanelType
} from 'office-ui-fabric-react/lib/Panel';
import { List } from 'office-ui-fabric-react/lib/List';
import { GroupDetails } from './GroupDetails';
import { ColorClassNames, FontClassNames } from '@uifabric/styling';
import './GroupList.scss';

export interface IGroupListState {
  items: Group[];
  selection: ISelection;
  canSelect?: string;
  showPanel: boolean;
}

export class Group implements IObjectWithKey {
  key: string;
  //  id: string;
  name: string;
  description: string;
  groupType: string;
  mailNickname: string;
  thumbnail: string;
  visibility: string;
  createdDate: Date;
  renewedDate: Date;
  policy: string;
  driveWebUrl: string;
  mailboxWebUrl: string;
  infoCard: string;
  driveRecentItems: DriveItem[];
  latestConversation: Conversation;
}

export class DriveItem {
  title: string;
  fileType: string;
  webUrl: string;
  thumbnailUrl: string;
}

export class Conversation {
  topic: string;
  lastDeliveredDateTime?: Date;
  lastDelivered: string;
  uniqueSenders: string[];
}


export class GroupList extends React.Component<{}, IGroupListState> {
  userId: string = "";
  private _hasMounted: boolean;

  constructor(props: any) {
    super(props);

    this._hasMounted = false;
    //this._onSelectionChanged = this._onSelectionChanged.bind(this);
    //this._onItemInvoked = this._onItemInvoked.bind(this);


    this.state = {
      items: [],
      selection: new Selection({ selectionMode: SelectionMode.single, onSelectionChanged: this._onSelectionChanged }),
      showPanel: false
    };
  }

  public componentDidMount() {
    this.userId = (window as any).userId;
    let data: Group[] = (window as any).groupData;
    let sel: ISelection = new Selection({ selectionMode: SelectionMode.single, onSelectionChanged: this._onSelectionChanged });
    sel.setItems(data, true);
    this.setState({
      items: data,
      selection: sel
    });

    for (let item of data) {
      this.getPicture(this.userId, item.key)
        .then((pictureUrl) => this.updateItemPhoto(item.key, pictureUrl.photoUrl));
    }

    this._hasMounted = true;
  }

  updateItemPhoto(id: string, photoUrl: string) {
    let items = [...this.state.items];
    let item = { ...items.filter(i => i.key === id)[0] };
    item.thumbnail = photoUrl;
    let index = items.map(function (e) { return e.key; }).indexOf(item.key);
    items[index] = item;
    this.setState({ items: items });
  }

  private getPicture(userId: string, id: string): Promise<any> {
    return fetch('/api/Group/Photo?id=' + id + '&userId=' + userId)
      .then((response) => response.json())
      .catch((reason) => {
        console.log(reason);
      });
  }

  getDetails(userId: string, id: string) {
    this.fetchDetails(userId, id)
      .then((group: Group) => {
        this.updateItemDetails(id, group);
      });
  }

  private fetchDetails(userId: string, id: string): Promise<Group> {
    return fetch('/api/Group/Details?id=' + id + '&userId=' + userId)
      .then((response) =>
        response.json())
      .catch((reason) => {
        console.log(reason);
      });
  }

  updateItemDetails(id: string, group: Group) {
    let items = [...this.state.items];
    let item = { ...items.filter(i => i.key === id)[0] };
    let index = items.map(function (e) { return e.key; }).indexOf(item.key);
    items[index] = { ...group };
    this.setState({ items: items });
    if (this._hasMounted) {
      this.forceUpdate();
    }
  }

  private _onSelectionChanged(): void {
    console.log("_onSelectionChanged");
    if (this._hasMounted) {
      this.forceUpdate();
    }
  }

  private _onItemInvoked = (item?: any, index?: number): void => {
    console.log('Item invoked', item, index);
    this.state.selection.setKeySelected(item.key, true, false);
    this.getDetails(this.userId, item.key);
    this.setState({ showPanel: true });
  }

  private _onClosePanel = (): void => {
    this.setState({ showPanel: false });
  }


  public render() {
    const { items, selection } = this.state;
    let selectedItem = {};
    if (selection.count > 0) {
      let selectedkey = selection.getSelection()[0].key;
      selectedItem = items.filter(i => i.key === selectedkey)[0];
    }
    return (
      <div>
        <Panel
          isOpen={this.state.showPanel}
          type={PanelType.smallFixedFar}
          onDismiss={this._onClosePanel}
        >
          <GroupDetails group={selectedItem as Group} />
        </Panel>
        <FocusZone direction={FocusZoneDirection.vertical}>
          <SelectionZone
            selection={selection}
            onItemInvoked={this._onItemInvoked}
          >
            <div className='ms-ListGhostingExample-container' data-is-scrollable={true}>
              <List
                items={items}
                onRenderCell={this._onRenderCell}
              />
            </div>
          </SelectionZone>
        </FocusZone>
      </div>
    );
  }

  private _onRenderCell(item?: any, index?: number, isScrolling?: boolean): JSX.Element {
    return (
      <div className='ms-ListGhostingExample-itemCell' data-is-focusable={true} data-selection-index={index} data-selection-invoke={true}>
        <Image
          className='ms-ListGhostingExample-itemImage'
          src={isScrolling ? undefined : item.thumbnail}
          width={50}
          height={50}
          imageFit={ImageFit.cover}
        />
        <div className='ms-ListGhostingExample-itemContent'>
          <div className='ms-ListGhostingExample-itemName ms-fontSize-l'>{item.name}</div>
          <div className='ms-ListGhostingExample-itemIndex'>{item.groupType} {(item.visibility) ? '(' + item.visibility + ')' : ''}</div>
        </div>
      </div>
    );
  }
}