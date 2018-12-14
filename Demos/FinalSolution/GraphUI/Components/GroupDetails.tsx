import * as React from 'react';
import * as ReactDOM from 'react-dom';

import { Group, IGroupListState, DriveItem } from './GroupList';
import {
    DocumentCard,
    DocumentCardActions,
    DocumentCardActivity,
    DocumentCardLocation,
    DocumentCardPreview,
    DocumentCardTitle,
    DocumentCardLogo,
    DocumentCardStatus,
    IDocumentCardPreviewProps,
    IDocumentCardLogoProps,
    DocumentCardType,
    IDocumentCardPreviewImage
} from 'office-ui-fabric-react/lib/DocumentCard';
import { ImageFit } from 'office-ui-fabric-react/lib/Image';
import { Icon, IconType, IIconProps } from 'office-ui-fabric-react/lib/Icon';
import { initializeFileTypeIcons, getFileTypeIconProps, FileIconType } from '@uifabric/file-type-icons';
import { GlobalSettings } from 'office-ui-fabric-react/lib/Utilities';
import { GroupCard } from './GroupCard';
import GroupList = require("./GroupList");
import Conversation = GroupList.Conversation;
initializeFileTypeIcons();

export interface IGroupDetailsProps {
    group: Group
}

export class GroupDetails extends React.Component<IGroupDetailsProps, any> {
    userId: string;

    constructor(props: IGroupDetailsProps) {
        super(props);
    }

    private getLibraryActivity(driveRecentItems: DriveItem[]): JSX.Element {
        if (driveRecentItems == null || driveRecentItems.length == 0) {
            return null;
        }

        let libraryActivity: JSX.Element = null;

        let globalSettings = (window as any).__globalSettings__;

        let recentDocs: IDocumentCardPreviewProps = {
            getOverflowDocumentCountText: (overflowCount: number) => `+${overflowCount} more`,
            previewImages: [
            ]
        };

        let documentCardDocTitle: JSX.Element = null;

        if (driveRecentItems.length == 1) {
            const doc = driveRecentItems[0];
            let iconProps: IIconProps = this.getIconProps((doc.fileType));
            let previewImage: IDocumentCardPreviewImage = {
                name: doc.title,
                url: doc.webUrl,
                previewImageSrc: doc.thumbnailUrl,
                iconSrc: globalSettings.icons[iconProps.iconName].code.props.src   // hack for file-type-icons
            };
            recentDocs.previewImages.push(previewImage);
            documentCardDocTitle = <DocumentCardTitle title={doc.title} shouldTruncate={true} />;
        }
        else {

            let docs = this.props.group.driveRecentItems;
            for (var i = 0; i < docs.length; i++) {
                let iconProps: IIconProps = {};
                switch (docs[i].fileType) {
                    case "folder":
                        iconProps = getFileTypeIconProps({ type: FileIconType.folder, size: 16 });
                        break;
                    default:
                        iconProps = getFileTypeIconProps({ extension: docs[i].fileType, size: 16 });
                        break;
                }

                let previewImage: IDocumentCardPreviewImage = {
                    name: docs[i].title,
                    url: docs[i].webUrl,
                    iconSrc: globalSettings.icons[iconProps.iconName].code.props.src   // hack for file-type-icons
                };
                recentDocs.previewImages.push(previewImage);
            }
        }

        libraryActivity = (
            <DocumentCard>
                <DocumentCardLogo logoIcon='OneDrive' />
                <DocumentCardTitle title='Latest Documents' />
                <DocumentCardPreview previewImages={recentDocs.previewImages} getOverflowDocumentCountText={recentDocs.getOverflowDocumentCountText} />
                {documentCardDocTitle}
                <DocumentCardLocation location='View Library' locationHref={this.props.group.driveWebUrl} />
            </DocumentCard>
        );

        return libraryActivity;
    }

    private getIconProps(fileSuffix: string): IIconProps {
        let iconProps: IIconProps = {};

        switch (fileSuffix) {
            case "folder":
                iconProps = getFileTypeIconProps({ type: FileIconType.folder, size: 16 });
                break;
            default:
                iconProps = getFileTypeIconProps({ extension: fileSuffix, size: 16 });
                break;
        }
        return iconProps;
    }

    private getMailboxActivity(latestConversation: Conversation, mailboxWebUrl: string): JSX.Element {
        let mailboxActivity = null;
        if (latestConversation) {
            let activityMessage = `Sent ${latestConversation.lastDelivered}`;
            let people = [];
            for (var i = 0; i < latestConversation.uniqueSenders.length; i++) {
                people.push({ name: latestConversation.uniqueSenders[i] });
            }
            mailboxActivity = (
                <DocumentCard>
                    <DocumentCardLogo logoIcon='OutlookLogo' />
                    <DocumentCardTitle title='Latest Conversation' shouldTruncate={true} />
                    <DocumentCardTitle title={latestConversation.topic} shouldTruncate={true} showAsSecondaryTitle={true} />
                    <DocumentCardActivity
                        activity={activityMessage}
                        people={people}
                    />
                    <DocumentCardLocation location='View Inbox' locationHref={mailboxWebUrl} ariaLabel='Group inbox' />
                </DocumentCard>
            );
        }
        return mailboxActivity;
    }

    public render() {
        const group = this.props.group;

        const libraryActivity: JSX.Element = this.getLibraryActivity(this.props.group.driveRecentItems);
        const mailboxActivity: JSX.Element = this.getMailboxActivity(this.props.group.latestConversation, this.props.group.mailboxWebUrl);

        const activity = (libraryActivity || mailboxActivity) ? (
            <div>
                <h2>Group Activity</h2>
                {libraryActivity}
                <br />
                {mailboxActivity}
            </div>
        ) : (null);

        return (
            <div>
                <h2>Group Information</h2>
                <DocumentCard>
                    <GroupCard group={this.props.group} />
                </DocumentCard>
                {activity}
            </div>
        );
    }

}