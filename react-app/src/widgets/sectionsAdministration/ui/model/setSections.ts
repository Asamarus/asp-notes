import { events } from '@/shared/config'
import { dispatchCrossTabEvent } from '@/shared/lib/useCrossTabEventListener'
import { setSections as setSectionsAction } from '@/entities/section'
import store from '@/shared/lib/store'

import type { Section } from '@/entities/section'
import type { components } from '@/shared/api'

function setSections(sections: components['schemas']['SectionsItemResponse'][]) {
  const payload: Section[] = []

  sections?.forEach((section) => {
    payload.push(section)
  })

  store.dispatch(setSectionsAction(payload))
  dispatchCrossTabEvent(events.sections.updated, payload)
}

export default setSections
