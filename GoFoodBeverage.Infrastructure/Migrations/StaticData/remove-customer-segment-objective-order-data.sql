DELETE FROM CustomerSegmentCondition WHERE ObjectiveId = 2 -- DELETE CustomerSegmentCondition with objective OrderData

DELETE CustomerSegment WHERE NOT EXISTS ( -- DELETE CustomerSegment which has no condition
  SELECT 1
  FROM CustomerSegmentCondition csc
  WHERE csc.CustomerSegmentId = CustomerSegment.Id
)