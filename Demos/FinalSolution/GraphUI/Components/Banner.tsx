import * as React from 'react';
import { ColorClassNames, FontClassNames } from '@uifabric/styling';
import './Banner.css';
import { IStyle } from '@uifabric/merge-styles/lib/IStyle';
import {
    Persona,
    PersonaInitialsColor,
    IPersonaStyles,
    IPersonaStyleProps,
    PersonaSize
    } from 'office-ui-fabric-react/lib/Persona';

export interface IBannerProps {
  name: string;
  email: string;
  imageUrl: string;
}

export class Banner extends React.Component<IBannerProps, {}> {
  constructor(props: IBannerProps) {
    super(props)
  }

    private getPersonaStyles(props: IPersonaStyleProps): Partial<IPersonaStyles> {
        return {
            root: {
                color: ColorClassNames.white,
                float: "right"
            },
            textContent: {
                color: ColorClassNames.white
            },
            primaryText: {
                color: ColorClassNames.white
            },
            secondaryText: {
                color: ColorClassNames.white
            }
        };
    }

  public render() {
      const persona = (this.props.name) ? (
          <Persona
              size={PersonaSize.size40}
              primaryText={this.props.name}
              secondaryText={this.props.email}
              imageUrl={this.props.imageUrl}
              getStyles={this.getPersonaStyles}
          />
      ) : (
          <span>&nbsp;</span>
      );

    return <div className='ms-Grid'>
      <div className={[
        'ms-Grid-row',
        'banner',
        ColorClassNames.themePrimaryBackground,
        ColorClassNames.white
      ].join(' ')}>
        <div className={[
          'ms-Grid-col ms-sm9',
          FontClassNames.xxLarge
        ].join(' ')}>
          Graph UI
        </div>
        <div className='ms-Grid-col ms-sm3 ms-textAlignRight'>
          {persona}
        </div>
      </div>
    </div>;
  }
}
