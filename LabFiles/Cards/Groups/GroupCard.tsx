import * as React from 'react';
import * as AdaptiveCards from "adaptivecards";
import { IGroupDetailsProps } from './GroupDetails';
import './GroupCard.scss';

export class GroupCard extends React.Component<IGroupDetailsProps, any> {
  constructor(props: IGroupDetailsProps) {
    super(props);
  }

  render() {
    let card = "";
    if (this.props.group.infoCard) {
      card = this.renderAdaptiveCard(this.props.group.infoCard);
    }
    return <div className="groupCard" dangerouslySetInnerHTML={{
      __html: card
    }} >
    </div>
  }

  private renderAdaptiveCard(card: any) {
    // Create an AdaptiveCard instance
    var adaptiveCard = new AdaptiveCards.AdaptiveCard();

    // Set its hostConfig property unless you want to use the default Host Config
    // Host Config defines the style and behavior of a card
    adaptiveCard.hostConfig = new AdaptiveCards.HostConfig({
      fontFamily: "Segoe UI, Helvetica Neue, sans-serif"
      // More host config options
    });

    // For markdown support you need a third-party library
    // E.g., to use markdown-it, include in your HTML page:
    //     <script type="text/javascript" src="https://unpkg.com/markdown-it/dist/markdown-it.js"></script>
    // And add this code to replace the default markdown handler:
    //     AdaptiveCards.processMarkdown = function(text) { return markdownit().render(text); }

    // Parse the card payload
    adaptiveCard.parse(card);

    // Render the card to an HTML element:
    var renderedCard = adaptiveCard.render();
    return renderedCard.innerHTML;
  }
}
