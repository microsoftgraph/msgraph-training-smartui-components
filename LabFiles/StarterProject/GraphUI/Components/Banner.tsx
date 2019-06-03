/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

import * as React from 'react';
import { ColorClassNames, FontClassNames } from '@uifabric/styling';
import './Banner.css';
import { IStyle } from '@uifabric/merge-styles/lib/IStyle';

export interface IBannerProps {
  name: string;
  email: string;
  imageUrl: string;
}

export class Banner extends React.Component<IBannerProps, {}> {
  constructor(props: IBannerProps) {
    super(props)
  }

  public render() {
    const persona = <span>&nbsp;</span>;

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
